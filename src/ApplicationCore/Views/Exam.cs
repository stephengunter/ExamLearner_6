using Infrastructure.Views;

namespace ApplicationCore.Views;

public class ExamViewModel : BaseRecordView
{
	public string? ExamType { get; set; }
	public string? RecruitExamType { get; set; }
	public string? OptionType { get; set; }


	public int RecruitId { get; set; }
	public int SubjectId { get; set; }


	public double Score { get; set; }
	public bool Reserved { get; set; }
	public string? Title { get; set; }
	public string? UserId { get; set; }

	public ICollection<ExamPartViewModel> Parts { get; set; } = new List<ExamPartViewModel>();
	public SubjectViewModel? Subject { get; set; }


	public string ExamStatusText => IsComplete ? "已完成" : "未完成";

	public bool IsComplete { get; set; }
}

public class ExamPartViewModel
{
	public int Id { get; set; }
	public string? Title { get; set; }
	public int ExamId { get; set; }
	public int Order { get; set; }
	public int OptionCount { get; set; }
	public bool MultiAnswers { get; set; }
	public double Points { get; set; }


	public ICollection<ExamQuestionViewModel> Questions { get; set; } = new List<ExamQuestionViewModel>();
}
