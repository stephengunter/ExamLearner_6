namespace ApplicationCore.Views;

public class OptionViewModel
{
	public int Id { get; set; }
	public string? Title { get; set; }
	public bool Correct { get; set; }
	public int QuestionId { get; set; }

	public ICollection<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();

}

