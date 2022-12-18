using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Entities;

namespace ApplicationCore.Models.Data;

[Table("DataExamSettings")]
public class ExamSettings : BaseDocument
{
	public int SubjectId { get; set; }
}
