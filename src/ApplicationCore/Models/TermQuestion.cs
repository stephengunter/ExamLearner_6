using Infrastructure.Interfaces;

namespace ApplicationCore.Models;

public class TermQuestion : IAggregateRoot
{
	public int TermId { get; set; }
	public int QuestionId { get; set; }
}
