using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class TermsSpecification : Specification<Term>
{
	public TermsSpecification()
	{
		Query.Where(item => !item.Removed)
			.Include(item => item.Subject);
	}
	public TermsSpecification(int id)
	{
		Query.Where(item => !item.Removed && item.Id == id)
			.Include(item => item.Subject);
	}
}

public class TermsBySubjectSpecification : Specification<Term>
{
	public TermsBySubjectSpecification(Subject subject)
	{
		Query.Where(item => !item.Removed && item.SubjectId == subject.Id)
			.Include(item => item.Subject);
	}

	public TermsBySubjectSpecification(Subject subject, int parentId)
	{
		Query.Where(item => !item.Removed && item.SubjectId == subject.Id && item.ParentId == parentId)
			.Include(item => item.Subject);
	}
	public TermsBySubjectSpecification(IList<int> subjectIds)
	{
		Query.Where(item => !item.Removed && subjectIds.Contains(item.SubjectId))
			.Include(item => item.Subject);
	}
	public TermsBySubjectSpecification(IList<int> subjectIds, int parentId)
	{
		Query.Where(item => !item.Removed && subjectIds.Contains(item.SubjectId) && item.ParentId == parentId)
			.Include(item => item.Subject);
	}
}
