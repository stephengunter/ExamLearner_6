using Infrastructure.Entities;

namespace ApplicationCore.Models;
public class Plan : BaseRecord, IBaseContract
{
	public string? Name { get; set; }

	public string? Description { get; set; }

	public decimal Money { get; set; }

	public int Discount { get; set; }

	public DateTime? StartDate { get; set; }

	public DateTime? EndDate { get; set; }

	public DateTime? ClearDate { get; set; }

	public bool Cleared => ClearDate.HasValue;

	public bool CanClear => Ended;

	public override bool Active => (Before == false && Ended == false);

	public bool Before => StartDate.HasValue ? DateTime.Now < StartDate.Value : false;

	public bool Ended => EndDate.HasValue ? DateTime.Now > EndDate.Value : false;

	public bool HasDateConflict(Plan model)
	{
		if (!this.StartDate.HasValue) return false;
		if (!model.StartDate.HasValue) return false;

		DateTime startDate = Convert.ToDateTime(this.StartDate);
		DateTime inputStartDate = Convert.ToDateTime(model.StartDate);

		DateTime endDate = DateTime.MaxValue;
		if (this.EndDate.HasValue) endDate = Convert.ToDateTime(this.EndDate);

		DateTime inputEndDate = DateTime.MaxValue;
		if (model.EndDate.HasValue) inputEndDate = Convert.ToDateTime(model.EndDate);

		if (inputStartDate >= startDate)
		{
			return inputStartDate <= endDate;
		}
		else
		{
			return inputEndDate >= startDate;
		}

	}
}
