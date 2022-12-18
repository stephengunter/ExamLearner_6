using ApplicationCore.Models;
namespace ApplicationCore.Exceptions;

public class PayWayNotFound : Exception
{
	public PayWayNotFound(string code) : base($"code: {code}")
	{

	}
}

public class PayRecordAlreadyExistWhilePay : Exception
{
	//(同一個Id)支付紀錄已經存在
	public PayRecordAlreadyExistWhilePay(string msg) : base(msg)
	{

	}
}

public class PayNotFound : Exception
{
	//支付時找不到Pay
	public PayNotFound(string code) : base($"code: {code}")
	{

	}
}

public class BillNotFoundWhilePay : Exception
{
	//支付時找不到Bill
	public BillNotFoundWhilePay(string msg) : base(msg)
	{

	}
}

public class BillPayedAlreadyWhilePay : Exception
{
	//Bill已經支付了
	public BillPayedAlreadyWhilePay(string msg) : base(msg)
	{

	}
}

public class PayMoneyNotEqualBillNeedPay : Exception
{
	//支付金額與帳單金額不相等
	public PayMoneyNotEqualBillNeedPay(string msg) : base(msg)
	{

	}
}

public class CreateThirdPartyTradeFailed : Exception
{
	//創建第三方支付交易失敗
	public CreateThirdPartyTradeFailed(Pay pay) : base($"billId: {pay.BillId}")
	{

	}
}

