using ApplicationCore.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;
public class BillViewModel : BaseRecordView
{
	public string UserId { get; set; } = string.Empty;

	public int PlanId { get; set; }

	public decimal Amount { get; set; }

	public bool HasDiscount { get; set; }

	public int PayWayId { get; set; }

	public DateTime? DeadLine { get; set; }

	public string DeadLineText => DeadLine.ToDateString();

	public bool Expired { get; set; }

	public bool Payed { get; set; }

	public decimal NeedPayMoney { get; set; }

	public decimal TotalPayed { get; set; }

	public DateTime? PayedDate { get; set; }

	public string PayedDateText => PayedDate.ToDateTimeString();

	public PlanViewModel? Plan { get; set; }

	public UserViewModel? User { get; set; }

	public ICollection<PayViewModel>? Pays { get; set; } = new List<PayViewModel>();

	public PayViewModel? PayInfo
	{
		get
		{
			if (Pays.IsNullOrEmpty()) return null;
			if (Payed) return null;
			return Pays!.Where(p => !p.Removed && !p.HasMoney && p.PayWay == PaymentTypes.ATM.ToString())
						.OrderByDescending(p => p.CreatedAt)
						.FirstOrDefault();
		}
	}

	public override string StatusText => Payed ? "已付款" : "未付款";

}
