using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class BillsSpecification : Specification<Bill>
{
	public BillsSpecification()
	{
		Query.Where(item => !item.Removed)
			.Include(item => item.User)
			.Include(item => item.Pays)
			.Include(item => item.Plan)
			.Include(item => item.Subscribes);
	}
	public BillsSpecification(int id)
	{
		Query.Where(item => !item.Removed && item.Id == id)
			.Include(item => item.User)
			.Include(item => item.Pays)
			.Include(item => item.Plan)
			.Include(item => item.Subscribes);
	}
	
	public BillsSpecification(User user)
	{
		Query.Where(item => !item.Removed && item.UserId == user.Id)
			.Include(item => item.User)
			.Include(item => item.Pays)
			.Include(item => item.Plan)
			.Include(item => item.Subscribes);
	}
	public BillsSpecification(User user, Plan plan)
	{
		Query.Where(item => !item.Removed && item.UserId == user.Id && item.PlanId == plan.Id)
			.Include(item => item.User)
			.Include(item => item.Pays)
			.Include(item => item.Plan)
			.Include(item => item.Subscribes);
	}
	
}

public class BillsByPlanSpecification: Specification<Bill>
{
	public BillsByPlanSpecification(Plan plan)
	{
		Query.Where(item => !item.Removed && item.PlanId == plan.Id);
	}
}
