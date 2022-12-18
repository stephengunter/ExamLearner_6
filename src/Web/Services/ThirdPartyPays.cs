using ApplicationCore.Settings;
using ApplicationCore.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ApplicationCore.Helpers;
using ApplicationCore.Exceptions;
using ApplicationCore.Views;
using System.Collections;
using ECPay.Payment.Integration.SPCheckOut;
using ECPay.Payment.Integration;

namespace Web.Services;

public interface IThirdPartyPayService
{
	EcPayTradeModel CreateEcPayTrade(Pay pay, int amount);
	TradeResultModel ResolveTradeResult(HttpRequest request);
}

public class EcPayService : IThirdPartyPayService
{
	private readonly EcPaySettings _ecpaySettings;
	private readonly AppSettings _appSettings;
	private readonly SubscribesSettings _subscribesSettings;
	private readonly ILogger<EcPayService> _logger;

	public EcPayService(IOptions<EcPaySettings> ecPaySettings, IOptions<AppSettings> appSettings,
		 IOptions<SubscribesSettings> subscribesSettings, ILogger<EcPayService> logger)
	{
		_ecpaySettings = ecPaySettings.Value;
		_appSettings = appSettings.Value;
		_subscribesSettings = subscribesSettings.Value;
		_logger = logger;
	}

	const string ATM_PAYWAY = "ATM";
	const string CREDIT_PAYWAY = "CREDIT";

	string ECPayUrl => _ecpaySettings.Url;
	string ECPayHashKey => _ecpaySettings.HashKey;
	string ECPayHashIV => _ecpaySettings.HashIV;
	string ECPayMerchantID => _ecpaySettings.MerchantID;

	string CreateTradeURL => $"{ECPayUrl}/SP/CreateTrade";
	string CheckOutURL => $"{ECPayUrl}/SP/SPCheckOut";
	string PayStoreUrl => $"{_appSettings.BackendUrl}/api/pays";

	string GetPaymentType(string type)
	{
		if (type.StartsWith(ATM_PAYWAY)) return ATM_PAYWAY;
		else if (type.StartsWith(CREDIT_PAYWAY)) return CREDIT_PAYWAY;
		else return "";
	}

	public EcPayTradeModel CreateEcPayTrade(Pay pay, int amount)
	{
		EcPayTradeSPToken? resultModel = null;

		try
		{
			using (SPCheckOutApi oPayment = new SPCheckOutApi())
			{
				oPayment.ServiceURL = CreateTradeURL;
				oPayment.HashKey = ECPayHashKey;
				oPayment.HashIV = ECPayHashIV;
				oPayment.Send.MerchantID = ECPayMerchantID;

				oPayment.Send.MerchantTradeNo = pay.Code;
				oPayment.Send.ItemName = "訂閱會員";   //商品名稱
				oPayment.Send.ReturnURL = PayStoreUrl;  //付款完成通知回傳網址
				oPayment.Send.TotalAmount = Convert.ToUInt32(amount);  //交易金額
				oPayment.Send.TradeDesc = _appSettings.Name!;  //交易描述

				oPayment.Send.NeedExtraPaidInfo = "N";  //額外回傳參數
				oPayment.Send.ClientBackURL = ""; //Client端返回特店的按鈕

				string info = $"CreateEcPayTrade: Payway = {pay.PayWay}, ReturnURL={oPayment.Send.ReturnURL}";
				if (pay.PayWay == ATM_PAYWAY)
				{
					oPayment.ATM.PaymentInfoURL = PayStoreUrl;
					oPayment.ATM.ExpireDate = _subscribesSettings.BillDaysToExpire;  //允許繳費有效天數

					info += $", PaymentInfoURL ={oPayment.ATM.PaymentInfoURL}";
				}

				_logger.LogInformation(info);

				string result = oPayment.Excute();
				_logger.LogInformation($"CreateEcPayTrade: result = {result}");

				try
				{
					resultModel = JsonConvert.DeserializeObject<EcPayTradeSPToken>(result);
				}
				catch (Exception ex)
				{
					_logger.LogError(new CreateEcPayTradeFailed(result, ex), result);
;					return new EcPayTradeModel();
				}

			}

			if (resultModel!.RtnCode!.ToInt() == 1)
			{
				//success
				return new EcPayTradeModel
				{
					TokenModel = resultModel,
					CheckOutURL = CheckOutURL,
					OriginURL = ECPayUrl,
				};
			}
			else
			{
				string msg = JsonConvert.SerializeObject(resultModel);
				//failed
				_logger.LogError(new CreateEcPayTradeFailed(msg), msg);

				return new EcPayTradeModel();
			}

		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.Message);
			return new EcPayTradeModel();
		}
	}


	public TradeResultModel ResolveTradeResult(HttpRequest request)
	{
		List<string> enErrors = new List<string>();
		Hashtable? htFeedback = null;

		try
		{
			using (var oPayment = new AllInOne())
			{
				oPayment.HashKey = ECPayHashKey;
				oPayment.HashIV = ECPayHashIV;
				/* 取回付款結果 */
				enErrors.AddRange(oPayment.CheckOutFeedback(request, ref htFeedback!));
			}

			if (enErrors.IsNullOrEmpty())
			{
				_logger.LogInformation($"htFeedback: {JsonConvert.SerializeObject(htFeedback)}");

				var tradeResultModel = new TradeResultModel()
				{
					Provider = _ecpaySettings.Id,
					Code = htFeedback["MerchantTradeNo"]!.ToString(),
					TradeNo = htFeedback["TradeNo"]!.ToString(),
					Amount = htFeedback["TradeAmt"]!.ToString()!.ToInt()
				};

				var rtnCode = htFeedback["RtnCode"]!.ToString()!.ToInt();
				if (rtnCode == 1) //付款成功
				{
					bool simulatePaid = false;
					if (htFeedback.ContainsKey("SimulatePaid")) simulatePaid = htFeedback["SimulatePaid"]!.ToString()!.ToInt() > 0;

					tradeResultModel.Simulate = simulatePaid; //是否為模擬的付款紀錄

					tradeResultModel.Payed = true;
					tradeResultModel.PayedDate = htFeedback["PaymentDate"]!.ToString();
					tradeResultModel.PayWay = GetPaymentType(htFeedback["PaymentType"]!.ToString()!);

				}
				else if (rtnCode == 2) //ATM 取號成功
				{

					tradeResultModel.Payed = false;
					tradeResultModel.PayWay = GetPaymentType(htFeedback["PaymentType"]!.ToString()!);
					tradeResultModel.BankCode = htFeedback["BankCode"]!.ToString();
					tradeResultModel.BankAccount = htFeedback["vAccount"]!.ToString();
					tradeResultModel.ExpireDate = htFeedback["ExpireDate"]!.ToString();
				}
				else
				{
					//Failed
					throw new EcPayTradeFeedBackFailed($"htFeedback: {JsonConvert.SerializeObject(htFeedback)}");
				}

				tradeResultModel.Data = JsonConvert.SerializeObject(htFeedback);
				return tradeResultModel;
			}
			else
			{
				//has error
				throw new EcPayTradeFeedBackError(String.Join("\\r\\n", enErrors));
			}
		}
		catch (Exception ex)
		{
			throw new EcPayTradeFeedBackError(ex.Message, ex);
		}
	}
}
