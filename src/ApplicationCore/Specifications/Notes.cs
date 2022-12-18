using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class NotesSpecification : Specification<Note>
{
	public NotesSpecification()
	{
		Query.Where(item => !item.Removed);
	}
}
public class NotesByParentSpecification : Specification<Note>
{
	public NotesByParentSpecification(int parentId)
	{
		Query.Where(item => !item.Removed && item.ParentId == parentId);
	}
}
public class NotesSubItemsSpecification : Specification<Note>
{
	public NotesSubItemsSpecification()
	{
		Query.Where(item => !item.Removed && item.ParentId > 0);
	}
}
public class NotesByTermsSpecification : Specification<Note>
{
	public NotesByTermsSpecification(IList<int> termIds)
	{
		Query.Where(item => !item.Removed && termIds.Contains(item.TermId))
			.Include(item => item.Term);
	}

	public NotesByTermsSpecification(Term term, int parentId)
	{
		Query.Where(item => !item.Removed && item.TermId == term.Id && item.ParentId == parentId)
			.Include(item => item.Term);
	}
}
