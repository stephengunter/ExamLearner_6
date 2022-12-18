using System.ComponentModel.DataAnnotations.Schema;
using Infrastructure.Entities;

namespace ApplicationCore.Models.Data;

[Table("DataNoteParams")]
public class NoteParams : BaseDocument
{
	public string? UserId { get; set; } = string.Empty;
}
