using ApplicationCore.Helpers;
using ApplicationCore.Views;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Models;
using Web.Models;

namespace Web.Controllers.Admin;

public class QuestionsController : BaseAdminController
{
	private readonly IDefaultRepository<Question> _questionsRepository;
	private readonly IDefaultRepository<Recruit> _recruitsRepository;
	private readonly IDefaultRepository<Subject> _subjectsRepository;
	private readonly IDefaultRepository<Term> _termsRepository;
	private readonly IDefaultRepository<UploadFile> _attachmentsRepository;
	private readonly IMapper _mapper;

	public QuestionsController(IDefaultRepository<Question> questionsRepository, IDefaultRepository<Recruit> recruitsRepository,
			IDefaultRepository<Subject> subjectsRepository, IDefaultRepository<Term> termsRepository, 
			IDefaultRepository<UploadFile> attachmentsRepository, IMapper mapper)

	{
		_questionsRepository = questionsRepository;
		_recruitsRepository = recruitsRepository;
		_subjectsRepository = subjectsRepository;
		_termsRepository = termsRepository;
		_attachmentsRepository = attachmentsRepository;

		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int subject, int term = 0, int recruit = 0, string keyword = "", int page = 1, int pageSize = 10)
	{
		if (page < 1)
		{
			var model = LoadOptionsAsync();
			return Ok(model);
		}

		var selectedSubject = await _subjectsRepository.FindSubjectLoadSubItemsAsync(subject);
		if (selectedSubject == null)
		{
			ModelState.AddModelError("subject", "科目不存在");
			return BadRequest(ModelState);
		}

		Term? selectedTerm = null;
		ICollection<int>? termIds = null;
		if (term > 0)
		{
			selectedTerm = await _termsRepository.FindTermLoadSubItemsAsync(term);
			if (selectedTerm == null)
			{
				ModelState.AddModelError("term", "條文不存在");
				return BadRequest(ModelState);
			}

			termIds = selectedTerm.GetSubIds();
			termIds.Add(selectedTerm.Id);
		}

		var allRecruits = await _recruitsRepository.FetchAllAsync();

		Recruit? selectedRecruit = null;
		List<int> recruitIds = new List<int>();
		if (recruit > 0)
		{
			selectedRecruit = allRecruits.FirstOrDefault(x => x.Id == recruit);

			if (selectedRecruit == null)
			{
				ModelState.AddModelError("recruit", "招考年度不存在");
				return BadRequest(ModelState);
			}

			recruitIds.Add(recruit);
			if (selectedRecruit.RecruitEntityType == RecruitEntityType.SubItem)
			{
				var partIds = allRecruits.Where(x => x.ParentId == recruit).Select(part => part.Id);
				recruitIds.AddRange(partIds.ToList());
				recruitIds.Add(recruit);
			}
		}

		var questions = await _questionsRepository.FetchAsync(selectedSubject, termIds, recruitIds);
		if (questions.IsNullOrEmpty()) return Ok(questions.GetPagedList(_mapper, page, pageSize));

		var keywords = keyword.GetKeywords();
		if (keywords.HasItems()) questions = questions.FilterByKeyword(keywords);


		var allTerms = await _termsRepository.FetchAllAsync();

		var types = new List<PostType>() { PostType.Question, PostType.Option };
		var attachments = await _attachmentsRepository.FetchByTypesAsync(types);

		var pagedList = questions.GetPagedList(_mapper, allRecruits.ToList(), attachments, allTerms.ToList(), page, pageSize);

		foreach (var item in pagedList.ViewList)
		{
			item.Options = item.Options.OrderByDescending(o => o.Correct).ToList();
		}

		return Ok(pagedList);

	}

	async Task<QuestionsAdminModel> LoadOptionsAsync()
	{
		//Subjects
		var subjects = await _subjectsRepository.FetchAsync();
		subjects = subjects.GetOrdered().ToList();
		_subjectsRepository.LoadSubItems(subjects);
		foreach (var item in subjects) item.GetSubIds();

		//Recruits
		var recruits = await _recruitsRepository.FetchAsync();
		recruits = recruits.GetOrdered().ToList();
		_recruitsRepository.LoadSubItems(recruits);

		return new QuestionsAdminModel(subjects.MapViewModelList(_mapper), recruits.MapViewModelList(_mapper));
	}

	[HttpGet("create")]
	public ActionResult Create(int subject, int term = 0)
	{
		var model = new QuestionViewModel() { SubjectId = subject };
		if (term > 0) model.TermIds = term.ToString();
		return Ok(model);
	}

	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] QuestionEditForm form)
	{
		var model = form.Question;
		await ValidateRequestAsync(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var question = model.MapEntity(_mapper, CurrentUserId);

		question = await _questionsRepository.AddAsync(question);
		if (question.Attachments!.HasItems())
		{
			foreach (var media in question.Attachments!)
			{
				media.PostType = PostType.Question;
				media.PostId = question.Id;
				media.SetCreated(CurrentUserId);
				await _attachmentsRepository.AddAsync(media);
			}
		}

		foreach (var option in question.Options)
		{
			foreach (var attachment in option.Attachments!)
			{
				attachment.PostType = PostType.Option;
				attachment.PostId = option.Id;
				attachment.SetCreated(CurrentUserId);
				await _attachmentsRepository.AddAsync(attachment);
			}
		}

		if (form.Choice && model.TermIds!.SplitToIds().HasItems())
		{
			int termId = model.TermIds!.SplitToIds().FirstOrDefault();
			var term = await _termsRepository.GetByIdAsync(termId);

			var qids = term!.QIds!.SplitToIds();
			qids.Add(question.Id);

			term.QIds = qids.JoinToStringIntegers();

			await _termsRepository.UpdateAsync(term);
		}


		return Ok(question.MapViewModel(_mapper));
	}

	[HttpGet("edit/{id}")]
	public async Task<ActionResult> Edit(int id)
	{
		var question = await _questionsRepository.FindQuestionIncludeItemsAsync(id);
		if (question == null) return NotFound();

		var allRecruits = await _recruitsRepository.FetchAllAsync();
		//選項的附圖

		var optionIds = question.Options.Select(x => x.Id).ToList();
		
		var questionAttachments = await _attachmentsRepository.FetchAsync(PostType.Question, id);
		var optionAttachments = await _attachmentsRepository.FetchAsync(PostType.Option, optionIds);

		var attachments = (questionAttachments ?? new List<UploadFile>()).Concat(optionAttachments ?? new List<UploadFile>());

		var model = question.MapViewModel(_mapper, allRecruits.ToList(), attachments.ToList());

		return Ok(model);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] QuestionViewModel model)
	{
		var question = await _questionsRepository.FindQuestionIncludeItemsAsync(id);
		if (question == null) return NotFound();

		await ValidateRequestAsync(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		question = model.MapEntity(_mapper, CurrentUserId, question);

		await _questionsRepository.UpdateAsync(question);
		if (question.Attachments!.HasItems())
		{
			foreach (var media in question.Attachments!)
			{
				media.PostType = PostType.Question;
				media.PostId = question.Id;
				if (media.Id > 0) media.SetUpdated(CurrentUserId);
				else media.SetCreated(CurrentUserId);
			}
		}
		
		await _attachmentsRepository.SyncAttachmentsAsync(PostType.Question, question, question.Attachments);

		foreach (var option in question.Options)
		{
			foreach (var attachment in option.Attachments!)
			{
				attachment.PostType = PostType.Option;
				attachment.PostId = option.Id;

				if (attachment.Id > 0) attachment.SetUpdated(CurrentUserId);
				else attachment.SetCreated(CurrentUserId);
			}

			await _attachmentsRepository.SyncAttachmentsAsync(PostType.Option, option, option.Attachments);
		}


		return Ok(question.MapViewModel(_mapper));
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		var question = await _questionsRepository.GetByIdAsync(id);
		if (question == null) return NotFound();

		question.Removed = true;
		question.SetUpdated(CurrentUserId);

		await _questionsRepository.UpdateAsync(question);

		return Ok();
	}

	async Task ValidateRequestAsync(QuestionViewModel model)
	{
		var subject = await _subjectsRepository.GetByIdAsync(model.SubjectId);
		if (subject == null) ModelState.AddModelError("subjectId", "科目不存在");

		if (model.Options.HasItems())
		{
			var correctOptions = model.Options.Where(item => item.Correct).ToList();
			if (correctOptions.IsNullOrEmpty()) ModelState.AddModelError("options", "必須要有正確的選項");
			else if (correctOptions.Count > 1)
			{
				if (!model.MultiAnswers) ModelState.AddModelError("options", "單選題只能有一個正確選項");

			}
		}

	}


}
