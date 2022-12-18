using Infrastructure.Views;

namespace ApplicationCore.Views;
public class ArticleViewModel : BaseRecordView
{
	public int CategoryId { get; set; }
	public string? Title { get; set; }
	public string? Content { get; set; }
	public int Clicks { get; set; }


	public string? Summary { get; set; }
}


