using Infrastructure.Views;

namespace ApplicationCore.Views;

public class CategoryViewModel : BaseRecordView
{
	public string? Key { get; set; }
	public string? Title { get; set; }


	public int ParentId { get; set; }
	public bool IsRootItem { get; set; }


}

