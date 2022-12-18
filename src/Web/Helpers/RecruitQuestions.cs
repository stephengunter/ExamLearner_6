using Infrastructure.Views;
using ApplicationCore.Helpers;
using ApplicationCore.Models;
using Web.Models;

namespace Web.Helpers;

public static class RecruitQuestionsViewService
{
	public static void LoadModeOptions(this RQIndexViewModel model)
	{
		model.ModeOptions = GetModeOptions();
	}
	public static void LoadSubjectOptions(this RQIndexViewModel model, IEnumerable<Subject> subjects, string emptyText = "")
	{
		var options = subjects.Select(item => item.ToOption()).ToList();

		if (!String.IsNullOrEmpty(emptyText)) options.Insert(0, new BaseOption<int>(-1, emptyText));

		model.SubjectOptions = options;

	}

	public static ICollection<BaseOption<int>> GetModeOptions()
	{
		var options = new List<BaseOption<int>>();
		foreach (RQMode mode in (RQMode[])Enum.GetValues(typeof(RQMode)))
		{
			string text = mode.GetDisplayName();
			if (!String.IsNullOrEmpty(text))
			{
				options.Add(new BaseOption<int>((int)mode, text));
			}

		}
		return options;
	}

	public static string GetDisplayName(this RQMode model)
	{
		if (model == RQMode.Read) return "閱讀";
		if (model == RQMode.Exam) return "測驗";
		return "";
	}

	public static RQMode ToRQModeType(this int val)
	{
		try
		{

			if (Enum.IsDefined(typeof(RQMode), val))
			{
				RQMode type = (RQMode)val;
				return type;
			}
			return RQMode.Unknown;


		}
		catch (Exception)
		{
			return RQMode.Unknown;
		}
	}
}
