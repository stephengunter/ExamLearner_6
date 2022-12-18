using ApplicationCore.Helpers;

namespace ApplicationCore.Views;
public class SubjectQuestionsViewModel
{
	public int SubjectId { get; set; }
	public ICollection<TermQuestionsViewModel> TermQuestions { get; set; } = new List<TermQuestionsViewModel>();

	public List<int> GetQuestionIds(IEnumerable<int>? except = null, IEnumerable<int>? mustIn = null)
	{
		if (TermQuestions.IsNullOrEmpty()) return new List<int>();

		var qids = TermQuestions.SelectMany(item => item.GetQuestionIds()).Distinct();

		if (except!.HasItems()) qids = qids.Except(except!);

		if (mustIn!.HasItems()) qids = qids.Where(x => mustIn!.Contains(x));

		return qids.ToList();
	}

	public List<int> GetRecruitQuestionIds(IEnumerable<int>? except = null)
	{
		if (TermQuestions.IsNullOrEmpty()) return new List<int>();

		var qids = TermQuestions.SelectMany(item => item.GetQuestionIds()).Distinct();
		if (except.IsNullOrEmpty()) return qids.ToList();

		return qids.Except(except!).ToList();
	}
}

public class TermQuestionsViewModel
{
	public int TermId { get; set; }
	public ICollection<int> QuestionIds { get; set; } = new List<int>();

	public List<int> GetQuestionIds()
	{
		if (SubItems.IsNullOrEmpty()) return QuestionIds.ToList();

		var subQids = SubItems!.SelectMany(item => item.GetQuestionIds());
		if (subQids.IsNullOrEmpty()) return QuestionIds.ToList();

		return QuestionIds.Union(subQids).Distinct().ToList();

	}


	public ICollection<TermQuestionsViewModel>? SubItems { get; set; }
}
