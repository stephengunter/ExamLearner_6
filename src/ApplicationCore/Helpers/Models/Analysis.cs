using ApplicationCore.Views;
using ApplicationCore.Models;
using AutoMapper;

namespace ApplicationCore.Helpers;

public static class AnalysisHelpers
{
	public static RecruitQuestionAnalysisView MapAnalysisView(this ExamSettingsViewModel examSettings)
	{
		var model = new RecruitQuestionAnalysisView();
		model.Recruit = examSettings.Recruit;
		model.RecruitId = examSettings.Recruit!.Id;

		var subjectList = examSettings.Parts!.FirstOrDefault()!.Subjects;
		model.SummaryList = subjectList.Select(item => new QuestionAnalysisSummaryView() { Subject = item.Subject, SubjectId = item.SubjectId }).ToList();

		foreach (var part in examSettings.Parts!)
		{
			var points = part.Points / part.Questions;
			foreach (var subject in part.Subjects)
			{
				var summaryView = model.SummaryList.FirstOrDefault(x => x.SubjectId == subject.SubjectId);
				summaryView!.QuestionCount += subject.TotalQuestions;
				summaryView.Points += points * subject.TotalQuestions;
			}
		}

		return model;

	}

	public static List<RecruitQuestionAnalysisView> MapAnalysisViewList(this List<ExamSettingsViewModel> examSettingsList)
		 => examSettingsList.Select(item => item.MapAnalysisView()).ToList();


	public static RecruitQuestionAnalysisView MapViewModel(this RecruitQuestionAnalysisView model,
		 Recruit recruit, ICollection<Subject> subjects, IMapper mapper)
	{
		model.Recruit = recruit.MapViewModel(mapper);
		model.Subject = subjects.FirstOrDefault(x => x.Id == model.SubjectId)!.MapViewModel(mapper);

		var subjectIds = model.SummaryList.Select(item => item.SubjectId).ToList();
		var selectedSubjects = subjects.Where(x => subjectIds.Contains(x.Id));

		var subjectViews = selectedSubjects.MapViewModelList(mapper);

		foreach (var summary in model.SummaryList)
		{
			summary.Subject = subjectViews.FirstOrDefault(x => x.Id == summary.SubjectId);
		}

		return model;
	}

	public static List<RecruitQuestionAnalysisView> MapToViewModelList(this IEnumerable<RecruitQuestionAnalysisView> models,
		 ICollection<Recruit> recruits, ICollection<Subject> subjects, IMapper mapper)
		 => models.Select(item => MapViewModel(item, recruits.FirstOrDefault(x => x.Id == item.RecruitId)!, subjects, mapper)).ToList();

}
