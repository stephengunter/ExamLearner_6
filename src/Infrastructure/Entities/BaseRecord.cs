namespace Infrastructure.Entities;
public abstract class BaseRecord : EntityBase
{
	public DateTime CreatedAt { get; set; } = DateTime.Now;
	public DateTime? LastUpdated { get; set; }
	public string? UpdatedBy { get; set; }

	public int Order { get; set; }
	public bool Removed { get; set; }

	public virtual bool Active => Order >= 0 && !Removed;

	public void SetCreated(string updatedBy)
	{
		this.CreatedAt = DateTime.Now;
		SetUpdated(updatedBy);
	}

	public void SetUpdated(string updatedBy)
	{
		this.UpdatedBy = updatedBy;
		this.LastUpdated = DateTime.Now;
	}
}
