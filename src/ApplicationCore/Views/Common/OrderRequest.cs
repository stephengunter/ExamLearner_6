namespace ApplicationCore.Views;

public class OrderRequest  //處理變更排序專用
{
	public int TargetId { get; set; }

	public int ReplaceId { get; set; }

	public bool Up { get; set; }
}
