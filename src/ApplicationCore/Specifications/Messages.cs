using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class MessagesSpecification : Specification<Message>
{
	public MessagesSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public MessagesSpecification(bool returned)
	{
		Query.Where(item => !item.Removed && item.Returned == returned);
	}
}
