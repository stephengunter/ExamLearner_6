using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class SubscribesSpecification : Specification<Subscribe>
{
	public SubscribesSpecification()
	{
		Query.Where(item => !item.Removed)
			.Include(item => item.Bill!.Pays)
			.Include(item => item.Bill!.Plan)
			.Include(item => item.User);
	}
	public SubscribesSpecification(int id)
	{
		Query.Where(item => !item.Removed && item.Id == id)
			.Include(item => item.Bill!.Pays)
			.Include(item => item.Bill!.Plan)
			.Include(item => item.User);
	}
	public SubscribesSpecification(Bill bill)
	{
		Query.Where(item => !item.Removed && item.BillId == bill.Id)
			.Include(item => item.Bill!.Pays)
			.Include(item => item.Bill!.Plan)
			.Include(item => item.User);
	}
	public SubscribesSpecification(Plan plan)
	{
		Query.Where(item => !item.Removed && item.PlanId == plan.Id)
			.Include(item => item.Bill!.Pays)
			.Include(item => item.Bill!.Plan)
			.Include(item => item.User);
	}
	public SubscribesSpecification(User user)
	{
		Query.Where(item => !item.Removed && item.UserId == user.Id)
			.Include(item => item.Bill!.Pays)
			.Include(item => item.Bill!.Plan)
			.Include(item => item.User);
	}
}
