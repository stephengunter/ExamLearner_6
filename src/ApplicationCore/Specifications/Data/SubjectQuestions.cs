using ApplicationCore.Models.Data;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.Data;
public class SubjectQuestionsSpecification : Specification<SubjectQuestions>
{
	public SubjectQuestionsSpecification(int subjectId)
	{
		Query.Where(item => item.SubjectId == subjectId);
	}
}
