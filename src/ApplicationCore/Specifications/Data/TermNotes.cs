using ApplicationCore.Models;
using ApplicationCore.Models.Data;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.Data;
public class TermNotesSpecification : Specification<TermNotes>
{
	public TermNotesSpecification(Subject subject)
	{
		Query.Where(item => item.SubjectId == subject.Id);
	}
	public TermNotesSpecification(Term term)
	{
		Query.Where(item => item.TermId == term.Id);
	}
}
