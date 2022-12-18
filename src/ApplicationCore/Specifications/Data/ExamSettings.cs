using ApplicationCore.Models.Data;
using Ardalis.Specification;

namespace ApplicationCore.Specifications.Data;
public class ExamSettingsSpecification : Specification<ExamSettings>
{
	public ExamSettingsSpecification(int subjectId)
	{
		Query.Where(item => item.SubjectId == subjectId);
	}
}
