using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;
public class Notice : BaseRecord
{
	public string Title { get; set; } = String.Empty;

	public string? Content { get; set; }

	public bool Top { get; set; }

	public int Clicks { get; set; }

	public bool Public { get; set; }

	public virtual ICollection<Receiver>? Receivers { get; set; } = new List<Receiver>();

}


public class Receiver : EntityBase
{
	public int NoticeId { get; set; }

	public string UserId { get; set; } = String.Empty;

	public DateTime? ReceivedAt { get; set; }

	[Required]
	public virtual Notice? Notice { get; set; }

	public virtual User? User { get; set; }


	public bool HasReceived => ReceivedAt.HasValue;
}
