using Infrastructure.Entities;

namespace ApplicationCore.Models;
public class ReviewRecord : BaseReviewRecord
{
	public ReviewableType Type { get; set; }
	public int PostId { get; set; }
}

public enum ReviewableType
{
	Resolve = 0,
	Unknown = -1
}

