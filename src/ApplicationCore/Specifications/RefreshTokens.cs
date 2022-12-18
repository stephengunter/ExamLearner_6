using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class RefreshTokensSpecification : Specification<RefreshToken>
{
	public RefreshTokensSpecification(User user)
	{
		Query.Where(item => item.UserId == user.Id);
	}
}
