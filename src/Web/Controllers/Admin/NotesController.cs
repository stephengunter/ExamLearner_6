using ApplicationCore.Models;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;

namespace Web.Controllers.Admin;

public class NotesController : BaseAdminController
{
	private readonly IDefaultRepository<Note> _notesRepository;
	private readonly IDefaultRepository<Subject> _subjectsRepository;
	private readonly IDefaultRepository<Term> _termsRepository;
	private readonly IDefaultRepository<Question> _questionsRepository;
	private readonly IDefaultRepository<UploadFile> _attachmentsRepository;
	private readonly IMapper _mapper;

	public NotesController(IDefaultRepository<Note> notesRepository, IDefaultRepository<Subject> subjectsRepository,
		IDefaultRepository<Term> termsRepository, IDefaultRepository<Question> questionsRepository,
		 IDefaultRepository<UploadFile> attachmentsRepository,  IMapper mapper)
	{
		_notesRepository = notesRepository;
		_subjectsRepository = subjectsRepository;
		_termsRepository = termsRepository;
		_attachmentsRepository = attachmentsRepository;
		_questionsRepository = questionsRepository;
		_mapper = mapper;
	}

	[HttpGet("categories")]
	public async Task<ActionResult> Categories(int type = 0)
	{
		var allSubjects = await _subjectsRepository.FetchAllAsync();
		var allTerms = await _termsRepository.FetchAllAsync();

		var rootSubjects = allSubjects.Where(x => x.ParentId == 0).GetOrdered();

		var categories = rootSubjects.Select(item => item.MapNoteCategoryViewModel()).ToList();
		foreach (var root in categories)
		{
			int parentId = root.Id;
			var subjects = allSubjects.Where(x => x.ParentId == parentId).GetOrdered();
			root.SubItems = subjects.Select(item => item.MapNoteCategoryViewModel(parentId)).ToList();
		}

		var subjectCategories = categories.SelectMany(x => x.SubItems);

		foreach (var subjectCategory in subjectCategories)
		{
			var terms = allTerms.Where(item => item.SubjectId == subjectCategory.Id && item.ParentId == 0 && item.ChapterTitle && !item.Hide).GetOrdered();
			if (type > 0) foreach (var item in terms) item.LoadSubItems(allTerms);

			subjectCategory.SubItems = terms.Select(item => item.MapNoteCategoryViewModel()).ToList();
		}

		return Ok(categories);
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int mode = 0, int term = 0, int subject = 0, string keyword = "")
	{
		if (term > 0)
		{
			var selectedTerm = await _termsRepository.FindTermLoadSubItemsAsync(term);
			if (selectedTerm == null)
			{
				ModelState.AddModelError("term", "條文課綱不存在");
				return BadRequest(ModelState);
			}

			var termViewModel = await LoadTermViewModelAsync(mode, selectedTerm);
			if (termViewModel.SubItems!.HasItems()) return Ok(termViewModel.SubItems);
			else return Ok(new List<TermViewModel> { termViewModel });
		}
		else if (subject > 0)
		{
			var keywords = keyword.GetKeywords();
			Subject? selectedSubject = await _subjectsRepository.GetByIdAsync(subject);
			int parent = -1;
			//科目底下所有條文
			var terms = await _termsRepository.FetchAsync(selectedSubject!, parent);
			var termIds = terms.Select(x => x.Id).ToList();

			if (terms.HasItems())
			{
				terms.LoadSubItems(_termsRepository.DbSet.AllSubItems().ToList());

				if (keywords.HasItems()) terms = terms.FilterByKeyword(keywords).ToList();
				terms = terms.GetOrdered().ToList();
			}

			var termViewModelList = new List<TermViewModel>();
			foreach (var item in terms)
			{
				var termViewModel = await LoadTermViewModelAsync(mode, item);
				termViewModelList.Add(termViewModel);
			}


			if (keywords.HasItems())
			{
				var notes = await _notesRepository.FetchAsync(termIds);
				notes = notes.FilterByKeyword(keywords).ToList();

				if (notes.HasItems())
				{
					foreach (int termId in notes.Select(x => x.TermId).Distinct())
					{
						var exist = termViewModelList.FirstOrDefault(x => x.Id == termId);
						if (exist == null)
						{
							var selectedTerm = await _termsRepository.FindTermLoadSubItemsAsync(term);
							var noteInTerms = notes.Where(x => x.TermId == termId);

							var termViewModel = await LoadTermViewModelAsync(mode, selectedTerm!);
							termViewModelList.Add(termViewModel);
						}
					}

					termViewModelList = termViewModelList.OrderBy(item => item.Order).ToList();

				}


			}


			return Ok(termViewModelList);
		}

		ModelState.AddModelError("params", "錯誤的查詢參數");
		return BadRequest(ModelState);
	}

	[HttpGet("create")]
	public async Task<ActionResult> Create(int term)
	{
		var selectedTerm = await _termsRepository.GetByIdAsync(term);
		if (selectedTerm == null)
		{
			ModelState.AddModelError("term", "條文課綱不存在");
			return BadRequest(ModelState);
		}
		int parent = 0;
		int maxOrder = await _notesRepository.GetMaxOrderAsync(selectedTerm, parent);
		var model = new NoteViewModel()
		{
			Active = true,
			Order = maxOrder + 1,
			TermId = term,
			ParentId = parent
		};
		return Ok(model);
	}

	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] NoteViewModel model)
	{
		ValidateRequest(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var note = model.MapEntity(_mapper, CurrentUserId);
		note = await _notesRepository.AddAsync(note);

		if (model.Attachments.HasItems())
		{
			var attachments = model.Attachments.Select(item => item.MapEntity(_mapper, CurrentUserId)).ToList();
			foreach (var attachment in attachments)
			{
				attachment.PostType = PostType.Note;
				attachment.PostId = note.Id;
			}

			attachments = (await _attachmentsRepository.AddRangeAsync(attachments)).ToList();
			note.Attachments = attachments;
		}

		return Ok(note.MapViewModel(_mapper));
	}

	[HttpGet("{id}")]
	public async Task<ActionResult> Details(int id)
	{
		var note = await _notesRepository.FindNoteLoadSubItemsAsync(id);
		if (note == null) return NotFound();

		return Ok(note.MapViewModel(_mapper));
	}

	[HttpGet("edit/{id}")]
	public async Task<ActionResult> Edit(int id)
	{
		var note = await _notesRepository.FindNoteLoadSubItemsAsync(id);
		if (note == null) return NotFound();

		var attachments = await _attachmentsRepository.FetchAsync(PostType.Note, id);

		return Ok(note.MapViewModel(_mapper, attachments));
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] NoteViewModel model)
	{
		var note = await _notesRepository.GetByIdAsync(id);
		if (note == null) return NotFound();

		ValidateRequest(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		note = model.MapEntity(_mapper, CurrentUserId, note);

		await _notesRepository.UpdateAsync(note);

		if (model.Attachments.HasItems())
		{
			var attachments = model.Attachments.Select(item => item.MapEntity(_mapper, CurrentUserId)).ToList();
			foreach (var attachment in attachments)
			{
				attachment.PostType = PostType.Note;
				attachment.PostId = note.Id;
			}

			await _attachmentsRepository.SyncAttachmentsAsync(PostType.Note, note, attachments);

			note.Attachments = attachments;
		}
		else
		{
			await _attachmentsRepository.SyncAttachmentsAsync(PostType.Note, note, null);
		}

		return Ok();
	}

	[HttpPost("order")]
	public async Task<ActionResult> Order([FromBody] OrderRequest model)
	{
		await _notesRepository.UpdateOrderAsync(model.TargetId, model.ReplaceId, model.Up);
		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		var note = await _notesRepository.GetByIdAsync(id);
		if (note == null) return NotFound();

		note.Removed = true;
		note.SetUpdated(CurrentUserId);
		await _notesRepository.UpdateAsync(note);

		return Ok();
	}

	async Task<TermViewModel> LoadTermViewModelAsync(int mode, Term term)
	{
		var termIds = new List<int>() { term.Id };
		if (term.SubItems!.HasItems()) termIds.AddRange(term.GetSubIds());

		var notes = await _notesRepository.FetchAsync(termIds);
		if (mode > 0) notes = notes.Where(x => x.Important).ToList();

		var postIds = notes.Select(x => x.Id).ToList();
		var attachments = await _attachmentsRepository.FetchAsync(PostType.Note, postIds);

		var noteViewList = notes.MapViewModelList(_mapper, attachments.ToList());

		var questionViewList = await FetchQuestionsByTermAsync(term);

		var termViewModel = term.MapViewModel(_mapper);
		termViewModel.LoadNotes(noteViewList);
		termViewModel.LoadQuestions(questionViewList);

		return termViewModel;
	}

	async Task<List<QuestionViewModel>> FetchQuestionsByTermAsync(Term selectedTerm)
	{
		var qIds = selectedTerm.GetQuestionIds();
		if (qIds.HasItems()) qIds = qIds.Distinct().ToList();

		var questions = await _questionsRepository.FetchByIdsAsync(qIds);

		return await LoadQuestionViewsAsync(questions);
	}

	async Task<List<QuestionViewModel>> LoadQuestionViewsAsync(IEnumerable<Question> questions)
	{
		List<Recruit>? recruits = null;
		List<Term>? allTerms = null;

		var postTypes = new List<PostType> { PostType.Question, PostType.Option };
		var attachments = await _attachmentsRepository.FetchByTypesAsync(postTypes);

		var models = questions.MapViewModelList(_mapper, recruits, attachments.ToList(), allTerms);
		return models;
	}

	void ValidateRequest(NoteViewModel model)
	{
		if (String.IsNullOrEmpty(model.Text) && model.Attachments.IsNullOrEmpty())
		{
			ModelState.AddModelError("text", "必須填寫內容");
			return;
		}
	}

}
