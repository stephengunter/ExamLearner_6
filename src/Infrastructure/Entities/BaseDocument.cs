namespace Infrastructure.Entities;

public abstract class BaseDocument : EntityBase
{
	public string Type { get; set; } = String.Empty;
	public string Content { get; set; } = String.Empty;  //json string
	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public DateTime LastUpdated { get; set; } = DateTime.Now;
}
