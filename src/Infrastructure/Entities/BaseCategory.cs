namespace Infrastructure.Entities;

public abstract class BaseCategory : BaseRecord
{
	public string Title { get; set; } = String.Empty;
	public int ParentId { get; set; }

	public bool IsRootItem => ParentId == 0;
}

