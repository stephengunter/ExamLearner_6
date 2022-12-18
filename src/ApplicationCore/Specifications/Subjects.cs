using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class SubjectsSpecification : Specification<Subject>
{
	public SubjectsSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public SubjectsSpecification(int parentId)
	{
		Query.Where(item => !item.Removed && item.ParentId == parentId);
	}
}
