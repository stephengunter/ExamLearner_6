using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Views;
using ApplicationCore.Services;

namespace Web.Controllers.Api;

[Authorize]
public class QuestionsController : BaseApiController
{
	private readonly IMapper _mapper;
	private readonly IDataService _dataService;
	private readonly IDefaultRepository<Term> _termsRepository;
	private readonly IDefaultRepository<Note> _notesRepository;
	private readonly IDefaultRepository<Question> _questionsRepository;
	private readonly IDefaultRepository<UploadFile> _attachmentsRepository;
	private readonly IDefaultRepository<Recruit> _recruitsRepository;

	public QuestionsController(IDataService dataService, IDefaultRepository<Term> termsRepository, IDefaultRepository<Note> notesRepository,
		IDefaultRepository<Question> questionsRepository, IDefaultRepository<Recruit> recruitsRepository,
		IDefaultRepository<UploadFile> attachmentsRepository, IMapper mapper)
	{
		_mapper = mapper;
		_dataService = dataService;
		_termsRepository = termsRepository;
		_notesRepository = notesRepository;
		_questionsRepository = questionsRepository;
		_attachmentsRepository = attachmentsRepository;
		_recruitsRepository = recruitsRepository;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int term = 0, int subject = 0)
	{
		var qIds = new List<int>();

		if (term > 0)
		{
			var termNotesView = await _dataService.FindTermNotesViewByTermAsync(new Term { Id = term });
			if (termNotesView == null) return NotFound();

			qIds.AddRange(termNotesView.RQIds!.SplitToIds());
			qIds.AddRange(termNotesView.QIds!.SplitToIds()!);
		}
		else if (subject > 0)
		{
			var termNotesViews = await _dataService.FetchTermNotesViewBySubjectAsync(new Subject { Id = subject });

			foreach (var termNotesView in termNotesViews!) qIds.AddRange(termNotesView.RQIds!.SplitToIds());
			foreach (var termNotesView in termNotesViews!) qIds.AddRange(termNotesView.QIds!.SplitToIds());

			qIds = qIds.OrderByDescending(x => x).ToList();
		}
		else
		{
			ModelState.AddModelError("params", "錯誤的查詢參數");
			return BadRequest(ModelState);
		}

		var result = new List<QuestionViewModel>();
		if (qIds.IsNullOrEmpty()) return Ok(result);


		qIds = qIds.Distinct().ToList();
		var questions = await _questionsRepository.FetchByIdsAsync(qIds);
		var viewList = await LoadQuestionViewsAsync(questions);

		foreach (var qId in qIds) result.Add(viewList.FirstOrDefault(x => x.Id == qId)!);

		return Ok(result);

	}


	async Task<List<QuestionViewModel>> LoadQuestionViewsAsync(IEnumerable<Question> questions)
	{
		var allRecruits = await _recruitsRepository.FetchAllAsync();
		List<Term>? allTerms = null;

		var types = new List<PostType> { PostType.Question, PostType.Option };
		var attachments = await _attachmentsRepository.FetchByTypesAsync(types);

		var viewList = questions.MapViewModelList(_mapper, allRecruits.ToList(), attachments.ToList(), allTerms);

		var sources = viewList.SelectMany(q => q.Resolves).SelectMany(r => r.Sources);
		foreach (var item in sources)
		{
			await item.MapContentAsync(_notesRepository, _termsRepository);
		}

		return viewList;
	}
}
