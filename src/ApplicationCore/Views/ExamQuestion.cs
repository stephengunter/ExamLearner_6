namespace ApplicationCore.Views;

public class ExamQuestionViewModel
{
	public int Id { get; set; }
	public int ExamPartId { get; set; }
	public int Order { get; set; }
	public int QuestionId { get; set; }
	public string? AnswerIndexes { get; set; }
	public string? OptionIds { get; set; } // 12,4,34,15

	public string? UserAnswerIndexes { get; set; }

	public QuestionViewModel? Question { get; set; }

	public ICollection<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();

	public ICollection<ResolveViewModel> Resolves { get; set; } = new List<ResolveViewModel>();

	public int Index { get; set; }
	public bool Correct { get; set; }
}
