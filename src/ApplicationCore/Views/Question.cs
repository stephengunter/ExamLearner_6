using Infrastructure.Views;

namespace ApplicationCore.Views;
public class QuestionViewModel : BaseRecordView
{
	public int SubjectId { get; set; }
	public string? Title { get; set; }

	public bool MultiAnswers { get; set; }

	public string? OptionsText { get; set; }
	public ICollection<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();

	public ICollection<ResolveViewModel> Resolves { get; set; } = new List<ResolveViewModel>();

	public ICollection<RecruitViewModel> Recruits { get; set; } = new List<RecruitViewModel>();

	public ICollection<AttachmentViewModel> Attachments { get; set; } = new List<AttachmentViewModel>();

	public string? RecruitsText { get; set; }

	public string? TermIds { get; set; }  //  Example: 1,6

	public ICollection<TermViewModel>? Terms { get; set; }

	public int Index { get; set; }


}

public class QuestionEditForm
{
	public QuestionViewModel Question { get; set; } = new QuestionViewModel();

	public bool Choice { get; set; } //精選

	public ICollection<TermViewModel>? Terms { get; set; }
}

