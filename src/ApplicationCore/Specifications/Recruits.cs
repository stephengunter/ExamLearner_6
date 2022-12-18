using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class RecruitsSpecification : Specification<Recruit>
{
	public RecruitsSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public RecruitsSpecification(int parentId)
	{
		Query.Where(item => !item.Removed && item.ParentId == parentId);
	}
}
