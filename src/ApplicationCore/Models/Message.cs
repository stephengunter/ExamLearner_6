using Infrastructure.Entities;

namespace ApplicationCore.Models;

public class Message : BaseRecord
{
	public string? Subject { get; set; }

	public string? Content { get; set; }

	public string? Email { get; set; }

	public string? ReturnContent { get; set; }//json string  => BaseMessageViewModel

	public bool Returned { get; set; }
}
