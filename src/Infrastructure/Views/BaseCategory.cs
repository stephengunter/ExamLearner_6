namespace Infrastructure.Views;
public class BaseCategoryView : BaseRecordView
{
	public string Title { get; set; } = String.Empty;

	public int ParentId { get; set; }
	public string? ParentTitle { get; set; }

	public bool IsRoot => ParentId == 0;
}
