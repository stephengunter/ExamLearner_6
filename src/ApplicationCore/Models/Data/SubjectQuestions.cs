using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Entities;

namespace ApplicationCore.Models.Data;

[Table("DataSubjectQuestions")]
public class SubjectQuestions : BaseDocument
{
	public int SubjectId { get; set; }
}
