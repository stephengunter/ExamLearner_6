using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models;

public class QuestionsAdminModel
{
	public QuestionsAdminModel(List<SubjectViewModel> subjects, List<RecruitViewModel> recruits)
	{
		Subjects = subjects;
		Recruits = recruits;
	}
	public List<SubjectViewModel> Subjects { get; }

	public List<RecruitViewModel> Recruits { get; }

	public PagedList<Question, QuestionViewModel>? PagedList { get; }
}
