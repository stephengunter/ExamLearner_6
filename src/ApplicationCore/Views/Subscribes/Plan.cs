using ApplicationCore.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;
public class PlanViewModel : BaseRecordView
{
	public int Discount { get; set; }

	public string? Name { get; set; }

	public string? Description { get; set; }

	public decimal Money { get; set; }

	public int Price { get; set; }

	public DateTime? StartDate { get; set; }

	public DateTime? EndDate { get; set; }

	public DateTime? ClearDate { get; set; }

	public string? StartDateText { get; set; }

	public string? EndDateText { get; set; }

	public string ClearDateText => ClearDate.ToDateTimeString();

	public bool CanClear { get; set; }

	public bool Before { get; set; }

	public bool Ended { get; set; }

	public bool HasDiscount => (decimal)Price < Money;

	public override string StatusText
	{
		get
		{
			if (Active) return "進行中";
			else if (Before) return "未開始";
			else if (Ended) return "已結束";
			return "";
		}
	}

}
