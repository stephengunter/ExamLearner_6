namespace Infrastructure.Entities;

public abstract class BaseReviewable : BaseRecord
{
	public bool Reviewed { get; set; }
}

public abstract class BaseReviewRecord : BaseRecord
{
	public bool Reviewed { get; set; }
}
