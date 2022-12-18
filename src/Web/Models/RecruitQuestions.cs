using ApplicationCore.Views;
using Infrastructure.Views;
using ApplicationCore.Helpers;

namespace Web.Models;

public enum RQMode
{
	Read = 0,
	Exam = 1,
	Unknown = -1
}

public class RQViewModel
{
	public string Title { get; set; } = String.Empty;

	public RecruitViewModel? Recruit { get; set; }

	public List<RQPartViewModel> Parts { get; set; } = new List<RQPartViewModel>();


	public void LoadTitle()
	{

		bool multiParts = Parts.Count > 1;
		for (int i = 0; i < Parts.Count; i++)
		{
			var part = Parts[i];
			int questionCount = part.Questions.Count;
			var pointsPerQuestion = questionCount > 0 ? (part.Points / questionCount) : 0;
			if (multiParts)
			{
				part.Title = $"第{(i + 1).ToCNNumber()}部份 - 共 {questionCount} 題 每題 {pointsPerQuestion} 分";
			}
			else
			{
				part.Title = $"共 {questionCount} 題 每題{pointsPerQuestion} 分";
			}
		}
	}
}

public class RQPartViewModel
{
	public int Order { get; set; }
	public string Title { get; set; } = String.Empty;
	public double Points { get; set; }
	public int OptionCount { get; set; }
	public string OptionType { get; set; } = String.Empty;
	public bool MultiAnswers { get; set; }

	public ICollection<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
}

public class RQIndexViewModel
{
	public ICollection<BaseOption<int>> ModeOptions { get; set; } = new List<BaseOption<int>>();
	public ICollection<BaseOption<int>> SubjectOptions { get; set; } = new List<BaseOption<int>>();

	public ICollection<RecruitViewModel> YearRecruits { get; set; } = new List<RecruitViewModel>();


}
