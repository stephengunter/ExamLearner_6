using Infrastructure.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;
public class RecruitQuestion : IAggregateRoot
{
	public int RecruitId { get; set; }
	[Required]
	public virtual Recruit? Recruit { get; set; }

	public int QuestionId { get; set; }
	[Required]
	public virtual Question? Question { get; set; }
}
