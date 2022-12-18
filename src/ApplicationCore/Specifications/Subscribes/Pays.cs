using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class PaysSpecification : Specification<Pay>
{
	public PaysSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public PaysSpecification(string code)
	{
		Query.Where(item => !item.Removed && item.Code.ToLower() == code.ToLower())
			.Include(item => item.Bill);
	}
	public PaysSpecification(int id)
	{
		Query.Where(item => !item.Removed && item.Id == id);
	}
	public PaysSpecification(PayWay payWay)
	{
		Query.Where(item => !item.Removed && item.PayWay.ToLower() == payWay.Code.ToLower());
	}
}
public class PaysByBillsSpecification : Specification<Pay>
{
	public PaysByBillsSpecification(ICollection<int> billIds)
	{
		Query.Where(item => !item.Removed && billIds.Contains(item.BillId));
	}
}
