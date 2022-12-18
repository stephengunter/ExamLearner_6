using Infrastructure.Views;

namespace ApplicationCore.Views;

public class NoteViewModel : BaseCategoryView
{
	public int TermId { get; set; }

	public string? Text { get; set; }

	public bool Important { get; set; }

	public string? Highlight { get; set; } //json string

	public string? Reference { get; set; } //json string

	public ICollection<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();

	public ICollection<ReferenceViewModel> References { get; set; } = new List<ReferenceViewModel>();

	public ICollection<string> Highlights { get; set; } = new List<string>();

	public bool HasKeyword(string keyword)
	{

		if (String.IsNullOrEmpty(this.Text)) return false;
		return this.Text.Contains(keyword);
	}
}

public class NoteParamsViewModel
{
	public int Mode { get; set; }

	public int SubjectId { get; set; }

	public int TermId { get; set; }
}


