using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;
using Infrastructure.Views;

namespace Web.Models;

public enum ExamStaus
{
	Reserved = 0,
	Completed = 1,
	Unknown = -1
}

public class ExamIndexViewModel
{
	//index

	public ICollection<BaseOption<int>> StatusOptions { get; set; } = new List<BaseOption<int>>();
	public ICollection<BaseOption<int>> SubjectOptions { get; set; } = new List<BaseOption<int>>();

	//create
	public ICollection<RecruitViewModel> YearRecruits { get; set; } = new List<RecruitViewModel>();

	public ICollection<BaseOption<int>> ExamTypeOptions { get; set; } = new List<BaseOption<int>>();

	public ICollection<BaseOption<int>> RecruitExamTypeOptions { get; set; } = new List<BaseOption<int>>();

	public PagedList<Exam, ExamViewModel>? PagedList { get; set; }
}
