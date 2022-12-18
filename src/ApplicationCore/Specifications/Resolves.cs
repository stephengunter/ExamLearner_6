using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class ResolvesSpecification : Specification<Resolve>
{
	public ResolvesSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public ResolvesSpecification(int questionId)
	{
		Query.Where(item => !item.Removed && item.QuestionId == questionId);
	}
	public ResolvesSpecification(Exam exam)
	{
		Query.Where(item => !item.Removed && exam.QuestionIds.Contains(item.QuestionId));
	}
}
