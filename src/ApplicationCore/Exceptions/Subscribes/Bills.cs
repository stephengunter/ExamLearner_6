using ApplicationCore.Models;

namespace ApplicationCore.Exceptions;

public class CreateWhileCurrentSubscribeExist : Exception
{
	//目前仍在訂閱期, 試圖建立新訂閱
	public CreateWhileCurrentSubscribeExist(string userId) : base($"userId: {userId}")
	{

	}
}

public class SelectedPlanDifferentFromActivePlan : Exception
{
	//試圖建立新訂閱時, 找不到指定的方案
	public SelectedPlanDifferentFromActivePlan(int selectedPlanId, int activePlanId) : base($"selectedPlanId: {selectedPlanId}  , activePlanId: {activePlanId}")
	{

	}
}

public class TryToCreateSecondValidBill : Exception
{
	//試圖建立第二張帳單
	public TryToCreateSecondValidBill(User user, Plan plan) : base($"userId: {user.Id}  , planId: {plan.Id}")
	{

	}
}


public class SelectedPriceDifferentFromActivePlan : Exception
{
	//指定的方案價格與ActivePlan不同
	public SelectedPriceDifferentFromActivePlan(int selectedPlanId, int activePlanId) : base($"selectedPlanId: {selectedPlanId}  , activePlanId: {activePlanId}")
	{

	}
}

public class NotPayedAfterPay : Exception
{
	//支付成功後,帳單仍然是未支付, 可能金額不對或支付際路錯誤
	public NotPayedAfterPay(Bill bill, Pay pay) : base($"billId: {bill.Id}  , payId: {pay.Id}")
	{

	}
}

