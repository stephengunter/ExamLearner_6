using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;

public class ExamsSpecification : Specification<Exam>
{
	public ExamsSpecification()
	{
		Query.Where(item => !item.Removed)
			.Include("Parts.Questions");
	}

	public ExamsSpecification(int id, bool withOptions = false)
	{
		if (withOptions)
		{
			Query.Where(item => !item.Removed && item.Id == id)
				.Include("Parts.Questions.Question.Options");
		}
		else
		{
			Query.Where(item => !item.Removed && item.Id == id)
				.Include("Parts.Questions.Question");
		}
	}

	public ExamsSpecification(User user)
	{
		Query.Where(item => !item.Removed && item.UserId == user.Id)
			.Include("Parts.Questions");
	}

}
