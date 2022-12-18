namespace ApplicationCore.Views;
public class EcPayTradeSPToken
{
	public string? RtnCode { get; set; }
	public string? RtnMsg { get; set; }
	public string? SPToken { get; set; }
	public string? MerchantID { get; set; }
	public string? MerchantTradeNo { get; set; }
	public string? CheckMacValue { get; set; }
}

public class EcPayTradeModel
{
	public EcPayTradeSPToken? TokenModel { get; set; }

	public string? OriginURL { get; set; }
	public string? CheckOutURL { get; set; }
	public string? PaymentType { get; set; }
	public bool HasToken => TokenModel != null && !String.IsNullOrEmpty(TokenModel.SPToken);
}

