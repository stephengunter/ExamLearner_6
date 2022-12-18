using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class ReceiversSpecification : Specification<Receiver>
{
	public ReceiversSpecification(int id)
	{
		Query.Where(item => item.Id == id)
			.Include(item => item.Notice);
	}
	public ReceiversSpecification(User user)
	{
		Query.Where(item => item.UserId == user.Id)
			.Include(item => item.Notice);
	}
}
