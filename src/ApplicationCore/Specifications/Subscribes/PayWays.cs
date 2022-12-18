using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class PayWaysSpecification : Specification<PayWay>
{
	public PayWaysSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public PayWaysSpecification(int id)
	{
		Query.Where(item => !item.Removed && item.Id == id);
	}
	public PayWaysSpecification(string code)
	{
		Query.Where(item => !item.Removed && item.Code.ToLower() == code.ToLower());
	}
}
