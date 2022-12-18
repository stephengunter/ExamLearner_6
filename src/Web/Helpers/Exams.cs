using Infrastructure.Views;
using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Web.Models;

namespace Web.Helpers;

public static class ExamsHelpers
{
	public static void LoadSubjectOptions(this ExamIndexViewModel model, IEnumerable<Subject> subjects, string emptyText = "")
	{
		var options = subjects.Select(item => item.ToOption()).ToList();

		if (!String.IsNullOrEmpty(emptyText)) options.Insert(0, new BaseOption<int>(-1, emptyText));

		model.SubjectOptions = options;

	}
	public static void LoadStatusOptions(this ExamIndexViewModel model, string emptyText = "全部")
	{
		var options = GetStatusOptions().ToList();

		if (!String.IsNullOrEmpty(emptyText)) options.Insert(0, new BaseOption<int>(-1, emptyText));

		model.StatusOptions = options;
	}
	public static void LoadExamTypeOptions(this ExamIndexViewModel model)
		 => model.ExamTypeOptions = GetExamTypeOptions();

	public static void LoadRecruitExamTypeOptions(this ExamIndexViewModel model)
		=> model.RecruitExamTypeOptions = GetRecruitExamTypeOptions();

	public static ICollection<BaseOption<int>> GetStatusOptions()
	{
		var options = new List<BaseOption<int>>();
		foreach (ExamStaus status in (ExamStaus[])Enum.GetValues(typeof(ExamStaus)))
		{
			string text = status.GetDisplayName();
			if (!String.IsNullOrEmpty(text))
			{
				options.Add(new BaseOption<int>((int)status, text));
			}

		}
		return options;
	}

	public static ICollection<BaseOption<int>> GetExamTypeOptions()
	{
		var options = new List<BaseOption<int>>();
		foreach (ExamType type in (ExamType[])Enum.GetValues(typeof(ExamType)))
		{
			string text = type.GetDisplayName();
			if (!String.IsNullOrEmpty(text))
			{
				options.Add(new BaseOption<int>((int)type, text));
			}

		}
		return options;
	}

	public static ICollection<BaseOption<int>> GetRecruitExamTypeOptions()
	{
		var options = new List<BaseOption<int>>();
		foreach (RecruitExamType type in (RecruitExamType[])Enum.GetValues(typeof(RecruitExamType)))
		{
			string text = type.GetDisplayName();
			if (!String.IsNullOrEmpty(text))
			{
				options.Add(new BaseOption<int>((int)type, text));
			}

		}
		return options;
	}


	public static string GetDisplayName(this ExamStaus status)
	{
		if (status == ExamStaus.Reserved) return "未完成";
		if (status == ExamStaus.Completed) return "已完成";
		return "";
	}

	public static string GetDisplayName(this ExamType type)
	{
		if (type == ExamType.Recruit) return "歷屆試題";
		if (type == ExamType.System) return "系統自訂";
		return "";
	}

	public static string GetDisplayName(this RecruitExamType type)
	{
		if (type == RecruitExamType.Exactly) return "完全相同";
		if (type == RecruitExamType.CrossYears) return "不限年度";
		return "";
	}

	public static ExamStaus ToExamStaus(this int val)
	{
		try
		{

			if (Enum.IsDefined(typeof(ExamStaus), val))
			{
				ExamStaus status = (ExamStaus)val;
				return status;
			}
			return ExamStaus.Unknown;


		}
		catch (Exception)
		{
			return ExamStaus.Unknown;
		}
	}

	public static ExamType ToExamType(this int val)
	{
		try
		{

			if (Enum.IsDefined(typeof(ExamType), val))
			{
				ExamType type = (ExamType)val;
				return type;
			}
			return ExamType.Unknown;


		}
		catch (Exception)
		{
			return ExamType.Unknown;
		}
	}

	public static RecruitExamType ToRecruitExamType(this int val)
	{
		try
		{

			if (Enum.IsDefined(typeof(RecruitExamType), val))
			{
				RecruitExamType type = (RecruitExamType)val;
				return type;
			}
			return RecruitExamType.Unknown;


		}
		catch (Exception)
		{
			return RecruitExamType.Unknown;
		}
	}

	public static IEnumerable<Exam> FilterByStatus(this IEnumerable<Exam> exams, ExamStaus staus)
	{
		if (staus == ExamStaus.Completed) return exams.Where(x => x.IsComplete);
		if (staus == ExamStaus.Reserved) return exams.Where(x => x.Reserved && !x.IsComplete);
		return exams;

	}
}
