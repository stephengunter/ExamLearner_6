using ApplicationCore.Models;

namespace ApplicationCore.Exceptions;

public class CreateSubscribeFailed : Exception
{
	//帳單支付後, 創建Subscribe失敗
	public CreateSubscribeFailed(Bill bill) : base($"billId: {bill.Id}")
	{

	}
}

public class BillPayedButNoCurrentSubscribe : Exception
{

	//有已繳帳單, 但是沒有CurrentSubscribe
	public BillPayedButNoCurrentSubscribe(User user, Plan plan) : base($"userId: {user.Id}  , planId: {plan.Id}")
	{

	}
}

