using ApplicationCore.Models;
using ApplicationCore.Services;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.Exceptions;
using ApplicationCore;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using Web.Models;
using ApplicationCore.DataAccess;

namespace Web.Services;

public interface ISubscribesService
{
	Task<Plan?> FindActivePlanAsync();

	Task<SubscribesIndexViewModel> GetSubscribesIndexViewAsync(string userId, bool allData = false);
	Task<BillEditForm> GetCreateBillFormAsync(string userId);
	Task<BillEditForm?> GetEditBillFormAsync(int billId, string userId);
	Task<BillViewModel?> GetBillDetailsViewAsync(int billId, string userId);

	Task<BillViewModel> StoreBillAsync(BillViewModel model, string userId);

	Task<EcPayTradeModel?> BeginPayAsync(int billId, BillViewModel model, string userId);
	TradeResultModel ResolveTradeResult(HttpRequest request);
	Task<Subscribe?> StorePayAsync(TradeResultModel model);

	Task<User> FindUserByIdAsync(string id);
}

public class SubscribesService : ISubscribesService
{
	private readonly IThirdPartyPayService _thirdPartyPayService;

	private readonly IDefaultRepository<Subscribe> _subscribesRepository;
	private readonly IDefaultRepository<Plan> _plansRepository;
	private readonly IDefaultRepository<Bill> _billsRepository;
	private readonly IDefaultRepository<Pay> _paysRepository;
	private readonly IDefaultRepository<PayWay> _paywaysRepository;
	private readonly IUsersService _usersService;

	private readonly SubscribesSettings _subscribesSettings;
	private readonly AdminSettings _adminSettings;
	private readonly ILogger<SubscribesService> _logger;
	private readonly IMapper _mapper;

	public SubscribesService(IThirdPartyPayService thirdPartyPayService,
		 IDefaultRepository<Subscribe> subscribesRepository,
		 IDefaultRepository<Plan> plansRepository, IDefaultRepository<Bill> billsRepository,
		 IDefaultRepository<Pay> paysRepository, IDefaultRepository<PayWay> paywaysRepository,
		 IUsersService usersService,
		 IOptions<SubscribesSettings> subscribesSettings, IOptions<AdminSettings> adminSettings,
		 ILogger<SubscribesService> logger, IMapper mapper)
	{

		_subscribesRepository = subscribesRepository;
		_plansRepository = plansRepository;
		_billsRepository = billsRepository;
		_paysRepository = paysRepository;
		_paywaysRepository = paywaysRepository;
		_thirdPartyPayService = thirdPartyPayService;
		_usersService = usersService;

		_subscribesSettings = subscribesSettings.Value;
		_adminSettings = adminSettings.Value;
		_logger = logger;
		_mapper = mapper;
	}

	public async Task<User> FindUserByIdAsync(string id) => await _usersService.FindByIdAsync(id);

	public async Task<SubscribesIndexViewModel> GetSubscribesIndexViewAsync(string userId, bool allData = false)
	{
		var subscribes = await _subscribesRepository.FetchByUserAsync(new User { Id = userId });

		var model = new SubscribesIndexViewModel();
		if (subscribes.HasItems())
		{
			subscribes = subscribes.GetOrdered();
			//User訂閱紀錄
			model.Records = subscribes.MapViewModelList(_mapper);
			
			//訂閱期之內(Active)的訂閱紀錄
			var activeSubscribe = subscribes.Where(item => item.Active).FirstOrDefault();
			if (activeSubscribe != null) model.Current = activeSubscribe.MapViewModel(_mapper);
		}

		if (model.Current == null)
		{
			//目前不在訂閱期
			var plan = await FindActivePlanAsync();
			if (plan != null)
			{
				bool canDiscount = subscribes.HasItems();
				model.Plan = plan.MapViewModel(_mapper, canDiscount);
			}
		}

		if (allData)
		{
			var bills = await _billsRepository.FetchByUserAsync(new User { Id = userId });
			if (bills.HasItems()) bills = bills.OrderByDescending(x => x.CreatedAt);

			model.Bills = bills.MapViewModelList(_mapper);

			var payways = await _paywaysRepository.FetchAsync();
			model.PayWays = payways!.MapViewModelList(_mapper);
		}

		return model;
	}

	public async Task<Plan?> FindActivePlanAsync()
	{
		bool active = true;
		var activePlans = await _plansRepository.FetchAsync(active);

		if (activePlans.IsNullOrEmpty())
		{
			//例外
			_logger.LogError(new NoActivePlanFound(), "NoActivePlanFound");

			return null;
		}
		else if (activePlans!.Count() > 1)
		{
			//例外
			_logger.LogError(new MutiActivePlanFound(), "MutiActivePlanFound");

			return activePlans!.GetOrdered().FirstOrDefault();
		}
		else return activePlans!.FirstOrDefault()!;
	}

	public async Task<BillEditForm> GetCreateBillFormAsync(string userId)
	{
		var indexModel = await GetSubscribesIndexViewAsync(userId);

		var model = new BillEditForm();

		var plan = indexModel.Plan;
		if (plan == null) return model; //正在訂閱期內或無方案可訂閱

		//查看是否已經有帳單未繳(同方案)
		var bills = await _billsRepository.FetchByUserAsync(new User { Id = userId }, new Plan { Id = plan.Id });
		if (bills.HasItems())
		{
			//帳單有繳的話, 應該在訂閱期內
			//所以應該只會有未繳的
			var unPayedBills = bills.Where(x => !x.Payed).ToList();
			if (unPayedBills.IsNullOrEmpty())
			{
				//沒有未繳帳單,異常
				//例外
				_logger.LogError(new BillPayedButNoCurrentSubscribe(new User { Id = userId }, new Plan { Id = plan.Id }), "BillPayedButNoCurrentSubscribe");
				return model;
			}
			else
			{
				//有未繳帳單, 找沒過期的
				var canPayBill = unPayedBills.Where(x => !x.Expired).FirstOrDefault();
				if (canPayBill != null)
				{
					model.Bill = canPayBill.MapViewModel(_mapper);
				}
			}
		}

		//只有進行到這裡,才可建立新訂單
		model.Plan = plan;

		var payways = await _paywaysRepository.FetchAsync();
		model.PayWays = payways!.GetOrdered().MapViewModelList(_mapper);

		if (model.Bill == null) model.Bill = new BillViewModel() { PayWayId = payways!.FirstOrDefault()!.Id };

		return model;
	}

	public async Task<BillEditForm?> GetEditBillFormAsync(int billId, string userId)
	{
		var bill = await _billsRepository.FindBillIncludeItemsAsync(billId);
		if (bill == null) return null;
		if (bill.Payed) return null; //已經支付, 應該去Details

		var form = new BillEditForm()
		{
			Bill = bill.MapViewModel(_mapper)
		};

		bool canPay = !bill.Expired;
		if (canPay)
		{
			var bills = await _billsRepository.FetchByUserAsync(new User { Id = userId }, new Plan { Id = bill.PlanId });
			if (bills.HasItems())
			{
				//查看是否已經有同方案, 已支付的帳單
				var payedBill = bills.Where(x => x.Payed).FirstOrDefault();
				if (payedBill != null) canPay = false;
			}
		}

		if (canPay)
		{
			var payways = await _paywaysRepository.FetchAsync();
			form.PayWays = payways!.GetOrdered().MapViewModelList(_mapper);
		}

		return form;
	}

	public async Task<BillViewModel?> GetBillDetailsViewAsync(int id, string userId)
	{
		var bill = await _billsRepository.FindBillIncludeItemsAsync(id);
		if (bill == null) return null;

		var payways = await _paywaysRepository.FetchAsync();

		var model = bill.MapViewModel(_mapper, payways);

		return model;
	}

	public async Task<BillViewModel> StoreBillAsync(BillViewModel model, string userId)
	{
		//驗證表單
		int planId = model.PlanId;
		var selectedPlan = await _plansRepository.GetByIdAsync(planId);
		if (selectedPlan == null) throw new EntityNotFoundException(new Plan { Id = planId });

		int payWayId = model.PayWayId;
		var selectedPayWay = await _paywaysRepository.GetByIdAsync(payWayId);
		if (selectedPayWay == null || !selectedPayWay.Active)
		{
			throw new EntityNotFoundException(new PayWay { Id = payWayId });
		}

		//取得訂閱紀錄
		var indexModel = await GetSubscribesIndexViewAsync(userId);

		var selectedPlanView = selectedPlan.MapViewModel(_mapper);
		//核對方案與金額
		var activePlanView = await CheckForExceptionsAsync(userId, indexModel, selectedPlanView);

		//進行到這裡表示通過驗證. 開始建立Bill
		var bill = new Bill
		{
			UserId = userId,
			PlanId = activePlanView.Id,
			Amount = activePlanView.Price,
			HasDiscount = activePlanView.HasDiscount,
			PayWayId = payWayId,
			DeadLine = DateTime.Today.AddDays(_subscribesSettings.BillDaysToExpire).ToEndDate()
		};

		bill = await _billsRepository.AddAsync(bill);

		return bill.MapViewModel(_mapper);
	}

	async Task<PlanViewModel> CheckForExceptionsAsync(string userId, SubscribesIndexViewModel indexModel, PlanViewModel selectedPlan)
	{
		if (indexModel.Current != null)
		{
			//目前仍在訂閱期
			throw new CreateWhileCurrentSubscribeExist(userId);

		}

		if (indexModel.Plan == null) throw new Exception();


		var activePlan = indexModel.Plan;

		if (selectedPlan.Id != activePlan.Id)
		{
			//找不到指定的方案
			throw new SelectedPlanDifferentFromActivePlan(selectedPlan.Id, activePlan.Id);
		}

		//查看是否已經有帳單未繳
		var bills = await _billsRepository.FetchByUserAsync(new User { Id = userId }, new Plan { Id = selectedPlan.Id });
		if (bills.HasItems())
		{
			//帳單有繳的話, 應該在訂閱期內
			//所以應該只會有未繳的
			var unPayedBills = bills.Where(x => !x.Payed).ToList();
			if (unPayedBills.IsNullOrEmpty())
			{
				//沒有未繳帳單,異常
				//例外
				_logger.LogError(new BillPayedButNoCurrentSubscribe(new User { Id = userId }, new Plan { Id = selectedPlan.Id }), "BillPayedButNoCurrentSubscribe");

			}
			else
			{
				//只有帳單過期, 才可建立新訂單
				var validBills = unPayedBills.Where(x => !x.Expired);
				if (validBills.HasItems())
				{
					//試圖建立第二張帳單
					throw new TryToCreateSecondValidBill(new User { Id = userId }, new Plan { Id = selectedPlan.Id });
				}
			}

		}


		if (selectedPlan.Price != activePlan.Price)
		{
			//金額不對
			throw new SelectedPriceDifferentFromActivePlan(selectedPlan.Id, activePlan.Id);
		}

		return activePlan;
	}

	public async Task<EcPayTradeModel?> BeginPayAsync(int billId, BillViewModel model, string userId)
	{
		// BeginPay 支付帳單
		var bill = await _billsRepository.FindBillIncludeItemsAsync(billId);
		if (bill == null) return null;

		int paywayId = model.PayWayId;
		var payway = await _paywaysRepository.GetByIdAsync(paywayId);
		if (payway == null) throw new EntityNotFoundException(new PayWay { Id = paywayId });

		if (bill.Payed) throw new BadRequestException(new RequestErrorViewModel { Key = "payed", Message = "此訂單已經支付過了" });

		if (bill.Expired) throw new BadRequestException(new RequestErrorViewModel { Key = "expired", Message = "訂單已過繳款期限" });

		var pay = Pay.Create(bill, payway, ThirdPartyPayment.EcPay);

		var amount = Convert.ToInt32(bill.NeedPayMoney);
		var ecPayTradeModel = _thirdPartyPayService.CreateEcPayTrade(pay, amount);

		if (!ecPayTradeModel.HasToken) throw new BadRequestException(new RequestErrorViewModel { Key = "", Message = "開啟支付程序失敗，請稍候再試." });

		await _paysRepository.AddAsync(pay);

		if (bill.PayWayId != paywayId)
		{
			bill.PayWayId = paywayId;
			await _billsRepository.UpdateAsync(bill);
		}

		ecPayTradeModel.PaymentType = payway.Code;

		return ecPayTradeModel;
	}

	public TradeResultModel ResolveTradeResult(HttpRequest request) => _thirdPartyPayService.ResolveTradeResult(request);


	public async Task<Subscribe?> StorePayAsync(TradeResultModel model)
	{
		var pay = await _paysRepository.FindByCodeAsync(model.Code);
		if (pay == null) throw new PayNotFound($"code: {model.Code}");

		if (model.Simulate) return await HandleSimulatePayAsync(pay, model);


		if (model.Payed) //付款成功的資料
		{
			if (pay.HasMoney) return null; //不處理重複發送的資料 

			pay.Money = Convert.ToDecimal(model.Amount);
			pay.PayedDate = model.PayedDate!.ToDatetimeOrNull();
			pay.TradeNo = model.TradeNo;
			pay.TradeData = model.Data;
			pay.Removed = false;

			await _paysRepository.UpdateAsync(pay);

			if (pay.Bill!.Removed)
			{
				pay.Bill.Removed = false;
				await _billsRepository.UpdateAsync(pay.Bill);
			}

			return await OnPayedAsync(pay);

		}
		else
		{
			if (String.IsNullOrEmpty(model.BankAccount)) return null;

			DateTime? expireDate = model.ExpireDate!.ToEndDate();
			// 獲取ATM虛擬帳號
			pay.BankCode = model.BankCode;
			pay.BankAccount = model.BankAccount;
			if (!String.IsNullOrEmpty(model.PayWay)) pay.PayWay = model.PayWay;

			await _paysRepository.UpdateAsync(pay);

			if (expireDate.HasValue)
			{
				pay.Bill!.DeadLine = expireDate;
				await _billsRepository.UpdateAsync(pay.Bill);
			}

			return null; //只獲取ATM虛擬帳號, 沒有付款完成 

		}
	}

	async Task<Subscribe?> HandleSimulatePayAsync(Pay pay, TradeResultModel model)
	{
		string userId = pay.Bill!.UserId;
		if (userId != _adminSettings.Id) return null;
		if (!model.Payed) return null;
		if (pay.HasMoney) return null;//不處理重複發送的資料

		pay.Money = Convert.ToDecimal(model.Amount);
		pay.PayedDate = model.PayedDate!.ToDatetimeOrNull();
		pay.TradeNo = model.TradeNo;
		pay.TradeData = model.Data;
		pay.Removed = false;

		await _paysRepository.UpdateAsync(pay);

		if (pay.Bill.Removed)
		{
			pay.Bill.Removed = false;
			await _billsRepository.UpdateAsync(pay.Bill);
		}

		return await OnPayedAsync(pay);
	}

	async Task<Subscribe> OnPayedAsync(Pay pay)  //當付款成功紀錄後執行
	{
		var bill = await _billsRepository.FindBillIncludeItemsAsync(pay.BillId);
		if (bill == null) throw new BillNotFoundWhilePay($"bill id: {pay.BillId}");

		if (!bill.Payed) throw new NotPayedAfterPay(bill, pay);

		bill.DeadLine = null;

		var payWay = await _paywaysRepository.FindByCodeAsync(pay.PayWay);
		if (payWay == null) _logger.LogError(new PayWayNotFound(pay.PayWay), "PayWayNotFound");
		else bill.PayWayId = payWay.Id;

		await _billsRepository.UpdateAsync(bill);

		//建立 Subscribe
		var subscribe = await _subscribesRepository.FindByBillAsync(bill);
		if (subscribe == null) subscribe = await _subscribesRepository.AddAsync(Subscribe.Create(bill));

		//加入角色
		if (subscribe.Active) await _usersService.AddSubscriberRoleAsync(subscribe.UserId);

		return subscribe;
	}

} //end of class

