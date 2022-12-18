using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using ApplicationCore.Views;
using ApplicationCore.DataAccess;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using ApplicationCore.Services;

namespace Web.Controllers.Admin;
public class DataController : BaseAdminController
{
	private readonly AdminSettings _adminSettings;
	private readonly IDataService _dataService;
	private readonly RootSubjectSettings _rootSubjectSettings;

	private readonly IDefaultRepository<Subject> _subjectsRepository;
	private readonly IDefaultRepository<Term> _termsRepository;
	private readonly IDefaultRepository<Note> _notesRepository;
	private readonly IDefaultRepository<UploadFile> _attachmentsRepository;
	private readonly IDefaultRepository<Recruit> _recruitsRepository;
	private readonly IDefaultRepository<Question> _questionsRepository;
	private readonly IMapper _mapper;

	public DataController(IOptions<RootSubjectSettings> rootSubjectSettings, IOptions<AdminSettings> adminSettings,
		IDataService dataService, IDefaultRepository<Subject> subjectsRepository, IDefaultRepository<Term> termsRepository, IDefaultRepository<Note> notesRepository,
		IDefaultRepository<Recruit> recruitsRepository, IDefaultRepository<Question> questionsRepository, IDefaultRepository<UploadFile> attachmentsRepository,
		IMapper mapper)
	{
		_adminSettings = adminSettings.Value;
		_rootSubjectSettings = rootSubjectSettings.Value;
		_subjectsRepository = subjectsRepository;
		_termsRepository = termsRepository;
		_notesRepository = notesRepository;
		_recruitsRepository = recruitsRepository;
		_questionsRepository = questionsRepository;
		_attachmentsRepository = attachmentsRepository;

		_dataService = dataService;
		_mapper = mapper;
	}


	//儲存每個Subject, Term 底下的QuestionId(精選試題)
	#region subject-questions

	[HttpPost("subject-questions")]
	public async Task<ActionResult> StoreSubjectQuestions([FromBody] AdminRequest model)
	{
		ValidateRequest(model, _adminSettings);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		//專業科目(1)：臺灣自然及人文地理
		var firstRootSubject = await _subjectsRepository.FindSubjectLoadSubItemsAsync(_rootSubjectSettings.FirstId);
		await SaveSubjectQuestionsAsync(firstRootSubject!);


		//專業科目(2)：郵政法規大意及交通安全常識
		var secondRootSubject = await _subjectsRepository.FindSubjectLoadSubItemsAsync(_rootSubjectSettings.SecondId);
		await SaveSubjectQuestionsAsync(secondRootSubject!);


		return Ok();
	}

	async Task SaveSubjectQuestionsAsync(Subject rootSubject)
	{
		var subjects = rootSubject.SubItems;

		var models = new List<SubjectQuestionsViewModel>();
		foreach (var subject in subjects!)
		{
			int parentId = 0;
			var terms = await _termsRepository.FetchAsync(subject, parentId);
			_termsRepository.LoadSubItems(terms);

			var subjectQuestionsModel = new SubjectQuestionsViewModel { SubjectId = subject.Id };
			foreach (var term in terms)
			{
				var termQuestionsModel = new TermQuestionsViewModel
				{
					TermId = term.Id,
					QuestionIds = term.QIds!.SplitToIds()
				};

				if (term.SubItems!.HasItems())
				{
					termQuestionsModel.SubItems = term.SubItems!.Select(subItem => new TermQuestionsViewModel
					{
						TermId = subItem.Id,
						QuestionIds = subItem.QIds!.SplitToIds()
					}).ToList();
				}

				subjectQuestionsModel.TermQuestions.Add(termQuestionsModel);

			}

			models.Add(subjectQuestionsModel);
		}

		await _dataService.SaveSubjectQuestionsAsync(rootSubject.Id, models);
	}
	#endregion




	[HttpPost("year-recruits")]
	public async Task<ActionResult> StoreYearRecruits([FromBody] AdminRequest model)
	{
		ValidateRequest(model, _adminSettings);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		int parent = 0;
		var recruits = await _recruitsRepository.FetchAsync(parent);

		recruits = recruits.Where(x => x.Active).GetOrdered();

		_recruitsRepository.LoadSubItems(recruits);

		var recruitViews = recruits.MapViewModelList(_mapper);

		foreach (var yearView in recruitViews)
		{
			foreach (var recruitView in yearView.SubItems!)
			{
				recruitView.QuestionIds = _questionsRepository.FetchQuestionIdsByRecruit(new Recruit { Id = recruitView.Id });

				foreach (var partView in recruitView.SubItems!)
				{
					partView.QuestionIds = _questionsRepository.FetchQuestionIdsByRecruit(new Recruit { Id = partView.Id });
				}
			}

		}


		await _dataService.SaveYearRecruitsAsync(recruitViews);

		return Ok();
	}

	[HttpPost("note-categories")]
	public async Task<ActionResult> NoteCategories([FromBody] AdminRequest model)
	{
		ValidateRequest(model, _adminSettings);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var allSubjects = await _subjectsRepository.FetchAllAsync();
		allSubjects = allSubjects.Where(x => x.Active);

		var allTerms = await _termsRepository.FetchAllAsync();

		var rootSubjects = allSubjects.Where(x => x.ParentId < 1).GetOrdered();

		var categories = rootSubjects.Select(item => item.MapNoteCategoryViewModel()).ToList();
		foreach (var root in categories)
		{
			int parentId = root.Id;
			var subjects = allSubjects.Where(x => x.ParentId == parentId).GetOrdered();
			root.SubItems = subjects.Select(item => item.MapNoteCategoryViewModel(parentId)).ToList();
		}

		var subjectCategories = categories.SelectMany(x => x.SubItems);

		//只到ChapterTitle, 捨棄Hide項目
		foreach (var subjectCategory in subjectCategories)
		{
			var terms = allTerms.Where(item => item.SubjectId == subjectCategory.Id && item.ParentId == 0 && item.ChapterTitle && !item.Hide).GetOrdered();
			subjectCategory.SubItems = terms.Select(item => item.MapNoteCategoryViewModel()).ToList();
		}

		await _dataService.SaveNoteCategoriesAsync(categories);

		return Ok();
	}

	#region term-notes

	[HttpPost("term-notes")]
	public async Task<ActionResult> TermNotes([FromBody] AdminRequest model)
	{
		ValidateRequest(model, _adminSettings);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		await _dataService.CleanTermNotesAsync();

		var categories = await _dataService.FetchNoteCategoriesAsync();
		var subjects = categories!.SelectMany(x => x.SubItems).ToList();

		var yearRecruits = await _dataService.FetchYearRecruitsAsync();
		var yearRecruitQids = new Dictionary<int, List<int>>();
		foreach (var yearRecruit in yearRecruits!)
		{
			var questionIds = new List<int>();
			foreach (var recruit in yearRecruit.SubItems!)
			{
				if (recruit.SubItems!.HasItems())
				{
					foreach (var part in recruit.SubItems!) questionIds.AddRange(part.QuestionIds!);
				}
				else
				{
					questionIds.AddRange(recruit.QuestionIds!);
				}
			}
			yearRecruitQids.Add(yearRecruit.Year, questionIds);
		}


		foreach (var subject in subjects)
		{
			if (subject.SubItems.HasItems())
			{
				foreach (var term in subject.SubItems)
				{
					var selectedTerm = await _termsRepository.FindTermLoadSubItemsAsync(term.Id);
					await SaveTermNotesAsync(selectedTerm!, yearRecruitQids);
				}

			}
			else
			{
				var selectedSubject = await _subjectsRepository.GetByIdAsync(subject.Id);
				int parent = -1;
				//科目底下所有條文
				var terms = (await _termsRepository.FetchAsync(selectedSubject!, parent)).Where(x => !x.ChapterTitle);
				var termIds = terms.Select(x => x.Id).ToList();

				if (terms.HasItems())
				{
					_termsRepository.LoadSubItems(terms);
					terms = terms.GetOrdered();

					foreach (var term in terms)
					{
						await SaveTermNotesAsync(term, yearRecruitQids);
					}
				}
			}
		}

		return Ok();
	}

	async Task SaveTermNotesAsync(Term term, Dictionary<int, List<int>> yearRecruitQids)
	{
		var termIds = new List<int>() { term.Id };
		if (term.SubItems!.HasItems()) termIds.AddRange(term.GetSubIds());
		var notes = await _notesRepository.FetchAsync(termIds);

		var RQIds = new List<int>();
		foreach (KeyValuePair<int, List<int>> yearRecruitQid in yearRecruitQids)
		{
			foreach (int qid in term.GetQuestionIds())
			{
				if (yearRecruitQid.Value.Contains(qid)) RQIds.Add(qid);
			}
		}
		RQIds = RQIds.Distinct().ToList();

		var qids = new List<int>();
		foreach (int qid in term.GetQuestionIds())
		{
			if (!RQIds.Contains(qid)) qids.Add(qid);
		}
		qids = qids.Distinct().ToList();

		var postIds = notes.Select(x => x.Id).ToList();
		var attachments = await _attachmentsRepository.FetchAsync(PostType.Note, postIds);

		var noteViewList = notes.MapViewModelList(_mapper, attachments.ToList());

		var termViewModel = term.MapViewModel(_mapper);


		await _dataService.SaveTermNotesAsync(termViewModel, noteViewList, RQIds, qids);

	}
	#endregion

}
