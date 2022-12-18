using Infrastructure.Views;

namespace ApplicationCore.Views;
public class RecruitViewModel : BaseCategoryView
{
	public string? EntityType { get; set; }

	public int Year { get; set; }

	public int SubjectId { get; set; }

	public string? DateText { get; set; }

	public bool Done { get; set; }

	public string? PS { get; set; }

	public int Points { get; set; }

	public int OptionCount { get; set; }

	public string? OptionType { get; set; }

	public bool MultiAnswers { get; set; }

	public string DoneText => this.Done ? "已結束" : "";

	public RecruitViewModel? Parent { get; set; }

	public ICollection<RecruitViewModel>? Parents { get; set; }

	public ICollection<RecruitViewModel>? SubItems { get; set; }

	public ICollection<int>? SubjectIds { get; set; }

	public ICollection<int>? QuestionIds { get; set; }

	public SubjectViewModel? Subject { get; set; }

}

public class RecruitEditForm
{
	public RecruitViewModel Recruit { get; set; } = new RecruitViewModel();

	public ICollection<BaseOption<int>>? SubjectOptions { get; set; }
}
