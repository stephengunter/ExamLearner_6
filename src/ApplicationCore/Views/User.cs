namespace ApplicationCore.Views;
public class UserViewModel
{
	public string? Id { get; set; }

	public string? Name { get; set; }

	public string? UserName { get; set; }

	public string? Email { get; set; }

	public DateTime CreatedAt { get; set; }

	public bool HasPassword { get; set; }

	public string CreatedAtText => CreatedAt.ToString("yyyy/MM/dd H:mm:ss");
	public string? Roles { get; set; }
}
