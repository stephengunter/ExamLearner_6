using ApplicationCore.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;

public class PayViewModel : BaseRecordView
{
	public string? Code { get; set; }

	public int BillId { get; set; }

	public decimal Money { get; set; }

	public DateTime? PayedDate { get; set; }

	public string? PayWay { get; set; }

	public string? PayWayTitle { get; set; }

	public string? TradeNo { get; set; }

	public string? BankCode { get; set; }

	public string? BankAccount { get; set; }

	public string? Provider { get; set; }

	public bool HasMoney { get; set; }

	public string? TradeData { get; set; } //json string

	public string PayedDateText => PayedDate.ToDateTimeString();

}

public class PayWayViewModel : BaseRecordView
{
	public string? Code { get; set; }

	public string? Title { get; set; }
}
