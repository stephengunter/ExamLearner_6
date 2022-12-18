using ApplicationCore.Models.Data;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.Data;
public class NoteParamsSpecification : Specification<NoteParams>
{
	public NoteParamsSpecification(string userId)
	{
		Query.Where(item => item.UserId == userId);
	}
}
