using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class OAuthSpecification : Specification<OAuth>
{
	public OAuthSpecification(User user, OAuthProvider provider)
	{
		Query.Where(item => item.UserId == user.Id && item.Provider == provider);
	}
}
