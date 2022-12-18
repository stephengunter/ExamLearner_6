using Infrastructure.Views;

namespace ApplicationCore.Views;

public class NoticeViewModel : BaseRecordView
{
	public string? Title { get; set; }

	public string? Content { get; set; }

	public bool Top { get; set; }

	public int Clicks { get; set; }

	public bool Public { get; set; }

}

public class ReceiverViewModel
{
	public int Id { get; set; }

	public int NoticeId { get; set; }

	public string? UserId { get; set; }

	public DateTime? ReceivedAt { get; set; }

	public bool HasReceived { get; set; }

	public NoticeViewModel? Notice { get; set; }
}


