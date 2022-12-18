
namespace ApplicationCore.Views;

public class RecruitQuestionAnalysisView
{
	public int RecruitId { get; set; }

	public RecruitViewModel? Recruit { get; set; }

	public int SubjectId { get; set; }

	public SubjectViewModel? Subject { get; set; }

	public List<QuestionAnalysisSummaryView> SummaryList { get; set; } = new List<QuestionAnalysisSummaryView>();

	public List<QuestionAnalysisDetailView> Details { get; set; } = new List<QuestionAnalysisDetailView>();

	public void LoadSummaryList()
	{
		SummaryList = new List<QuestionAnalysisSummaryView>();
		foreach (var detail in Details)
		{
			var summary = SummaryList.FirstOrDefault(x => x.SubjectId == detail.SubjectId);
			if (summary == null)
			{
				summary = new QuestionAnalysisSummaryView() { SubjectId = detail.SubjectId };
				summary.AddDetail(detail);
				SummaryList.Add(summary);
			}
			else
			{
				summary.AddDetail(detail);
			}
		}
	}

}

public class QuestionAnalysisSummaryView
{
	public int SubjectId { get; set; }
	public SubjectViewModel? Subject { get; set; }
	public int QuestionCount { get; set; }
	public double Points { get; set; }


	public void AddDetail(QuestionAnalysisDetailView detail)
	{
		QuestionCount += 1;
		Points += detail.Points;
	}
}

public class QuestionAnalysisDetailView
{
	public int QuestionId { get; set; }
	public int SubjectId { get; set; }
	public double Points { get; set; }
}
