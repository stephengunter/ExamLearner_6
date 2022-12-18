using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class PlansSpecification : Specification<Plan>
{
	public PlansSpecification()
	{
		Query.Where(item => !item.Removed);
	}
}
