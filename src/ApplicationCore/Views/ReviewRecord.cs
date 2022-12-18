using Infrastructure.Views;

namespace ApplicationCore.Views;

public class ReviewRecordViewModel : BaseReviewableView
{
	public int PostId { get; set; }

	public string? Type { get; set; }
}
