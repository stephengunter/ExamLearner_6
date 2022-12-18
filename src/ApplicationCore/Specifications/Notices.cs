using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class NoticesSpecification : Specification<Notice>
{
	public NoticesSpecification()
	{
		Query.Where(item => !item.Removed)
			.Include(item => item.Receivers);
	}
	public NoticesSpecification(bool isPublic)
	{
		Query.Where(item => !item.Removed && item.Public == isPublic)
			.Include(item => item.Receivers);
	}
}
