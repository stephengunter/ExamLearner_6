using Infrastructure.Entities;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Models;

public class RefreshToken : EntityBase
{
	public string UserId { get; set; } = String.Empty;
	public string Token { get; set; } = String.Empty;
	public DateTime Expires { get; set; }

	public string? RemoteIpAddress { get; set; }

	[Required]
	public virtual User? User { get; set; }

	public bool Active => DateTime.UtcNow <= Expires;
}
