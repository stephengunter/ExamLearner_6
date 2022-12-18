using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;

public class QuestionsSpecification : Specification<Question>
{
	public QuestionsSpecification()
	{
		Query.Where(item => !item.Removed)
			.Include(item => item.Options)
			.Include(item => item.Resolves)
			.Include("RecruitQuestions.Recruit");
	}

	public QuestionsSpecification(int id)
	{
		Query.Where(item => !item.Removed && item.Id == id)
			.Include(item => item.Options)
			.Include(item => item.Resolves)
			.Include("RecruitQuestions.Recruit");
	}

	public QuestionsSpecification(IEnumerable<int> ids)
	{
		Query.Where(item => !item.Removed && ids.Contains(item.Id))
			.Include(item => item.Options)
			.Include(item => item.Resolves)
			.Include("RecruitQuestions.Recruit");
	}
	
}

public class QuestionsBySubjectsSpecification : Specification<Question>
{
	public QuestionsBySubjectsSpecification(Subject subject)
	{
		Query.Where(item => !item.Removed && item.SubjectId == subject.Id)
			.Include(item => item.Options)
			.Include(item => item.Resolves)
			.Include("RecruitQuestions.Recruit");
	}
	public QuestionsBySubjectsSpecification(ICollection<int> subjectIds)
	{
		Query.Where(item => !item.Removed && subjectIds.Contains(item.SubjectId))
			.Include(item => item.Options)
			.Include(item => item.Resolves)
			.Include("RecruitQuestions.Recruit");
	}
}
