using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using Web.Models;

namespace Web.Controllers.Api;

[Authorize(Policy = "Subscriber")]
public class NotesController : BaseApiController
{
	private readonly IDataService _dataService;
	private readonly IDefaultRepository<Note> _notesRepository;
	private readonly IMapper _mapper;

	public NotesController(IDataService dataService, IDefaultRepository<Note> notesRepository,
		 IMapper mapper)
	{
		_dataService = dataService;
		_notesRepository = notesRepository;
		_mapper = mapper;
	}

	[HttpGet("categories")]
	public async Task<ActionResult> Categories()
	{
		var categories = await _dataService.FetchNoteCategoriesAsync();

		var paramsView = await _dataService.FindNoteParamsAsync(CurrentUserId);

		var model = new NotesIndexModel(categories!.ToList(), paramsView!);

		return Ok(model);
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int mode = 0, int term = 0, int subject = 0, string keyword = "")
	{
		if (term > 0)
		{
			var termViewModel = await _dataService.FindTermNotesByTermAsync(new Term { Id = term });
			if (termViewModel == null) return NotFound();

			if (mode < 0 || mode > 1) mode = 0;
			await _dataService.SaveNoteParamsAsync(CurrentUserId, new NoteParamsViewModel
			{
				Mode = mode,
				SubjectId = 0,
				TermId = term
			});

			if (termViewModel.SubItems!.HasItems()) return Ok(termViewModel.SubItems);
			else return Ok(new List<TermViewModel> { termViewModel });
		}
		else if (subject > 0)
		{
			var keywords = keyword.GetKeywords();
			var termViewList = await _dataService.FetchTermNotesBySubjectAsync(new Subject { Id = subject });

			if (keywords.IsNullOrEmpty())
			{
				if (mode < 0 || mode > 1) mode = 0;
				await _dataService.SaveNoteParamsAsync(CurrentUserId, new NoteParamsViewModel
				{
					Mode = mode,
					SubjectId = subject,
					TermId = 0
				});

				return Ok(termViewList);
			}


			var termsHasKeywords = FilterByKeywords(termViewList!, keywords);

			var noteIds = FetchNoteIdsByKeywords(termViewList!, keywords);

			var resultList = new List<TermViewModel>();
			if (termsHasKeywords.HasItems()) resultList.AddRange(termsHasKeywords);


			var terms = termViewList!.SelectMany(x => x.SubItems!).ToList();
			if (terms.IsNullOrEmpty()) terms = termViewList!.ToList();

			foreach (var termView in terms)
			{
				termView.Notes = termView.Notes!.Where(x => noteIds.Contains(x.Id)).ToList();
				if (termView.Notes.HasItems())
				{
					if (resultList.FirstOrDefault(x => x.Id == termView.Id) == null)
					{
						resultList.Add(termView);
					}
				}
			}

			return Ok(resultList);
		}

		ModelState.AddModelError("params", "錯誤的查詢參數");
		return BadRequest(ModelState);
	}

	List<TermViewModel> FilterByKeywords(IEnumerable<TermViewModel> termViewList, IList<string> keywords)
	{
		var terms = termViewList.SelectMany(x => x.SubItems!).ToList();
		if (terms.HasItems())
		{
			return terms.Where(item => keywords.Any(item.Text!.Contains)).ToList();
		}
		else
		{

			return termViewList.Where(item => keywords.Any(item.Text!.Contains)).ToList();
		}
	}

	List<int> FetchNoteIdsByKeywords(IEnumerable<TermViewModel> termViewList, IList<string> keywords)
	{
		var terms = termViewList.SelectMany(x => x.SubItems!).ToList();
		if (terms.HasItems())
		{
			return terms.SelectMany(x => x.Notes!)
						.Where(item => keywords.Any(item.HasKeyword))
						.Select(x => x.Id).ToList();
		}
		else
		{
			return termViewList.SelectMany(x => x.Notes!)
								.Where(item => keywords.Any(item.HasKeyword))
								.Select(x => x.Id).ToList();
		}

	}

	[HttpGet("{id}")]
	public async Task<ActionResult> Details(int id)
	{
		var note = await _notesRepository.FindNoteLoadSubItemsAsync(id);
		if (note == null) return NotFound();

		return Ok(note.MapViewModel(_mapper));
	}

}


