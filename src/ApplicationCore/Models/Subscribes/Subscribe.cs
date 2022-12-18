using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;
public class Subscribe : BaseRecord, IBaseContract
{
	public string UserId { get; set; } = String.Empty;

	public int BillId { get; set; }

	public int PlanId { get; set; }

	public DateTime? StartDate { get; set; }

	public DateTime? EndDate { get; set; }

	[Required]
	public virtual User? User { get; set; }

	[Required]
	public virtual Bill? Bill { get; set; }

	public static Subscribe Create(Bill bill)
	{
		var plan = bill.Plan;

		return new Subscribe
		{
			BillId = bill.Id,
			UserId = bill.UserId,
			PlanId = bill.PlanId,
			StartDate = DateTime.Now > plan!.StartDate ? DateTime.Now : plan.StartDate,
			EndDate = plan.EndDate
		};
	}


	public override bool Active => (Before == false && Ended == false);

	public bool Before => StartDate.HasValue ? DateTime.Now < StartDate.Value : false;

	public bool Ended => EndDate.HasValue ? DateTime.Now > EndDate.Value : false;
}
