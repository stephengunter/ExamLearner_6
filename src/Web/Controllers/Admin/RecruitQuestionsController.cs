using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Models;

namespace Web.Controllers.Admin;

public class RecruitQuestionsController : BaseAdminController
{
	private readonly IDefaultRepository<Question> _questionsRepository;
	private readonly IDefaultRepository<Recruit> _recruitsRepository;
	private readonly IDefaultRepository<Subject> _subjectsRepository;
	private readonly IDefaultRepository<Term> _termsRepository;
	private readonly IDefaultRepository<UploadFile> _attachmentsRepository;
	private readonly IMapper _mapper;

	public RecruitQuestionsController(IDefaultRepository<Question> questionsRepository, IDefaultRepository<Recruit> recruitsRepository,
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
	public async Task<ActionResult> Index(int recruit)
	{
		var selectedRecruit = await _recruitsRepository.FindRecruitLoadSubItemsAsync(recruit);
		if (selectedRecruit == null)
		{
			ModelState.AddModelError("recruit", "年度不存在");
			return BadRequest(ModelState);
		}

		if (selectedRecruit.RecruitEntityType == RecruitEntityType.Year) return Ok(new List<Question>().GetPagedList(_mapper));

		var allSubjects = await _subjectsRepository.FetchAllAsync();

		var subject = await _recruitsRepository.FindSubjectAsync(selectedRecruit, allSubjects);

		if (subject == null)
		{
			ModelState.AddModelError("subject", "科目不存在");
			return BadRequest(ModelState);
		}
		
		subject.LoadSubItems(_subjectsRepository.DbSet.AllSubItems().ToList());

		
		var recruitIds = selectedRecruit.GetSubIds();
		recruitIds.Add(selectedRecruit.Id);
		var questions = await _questionsRepository.FetchAsync(subject, termIds:null, recruitIds);

		List<Term>? allTerms = null;
		List<Recruit>? allRecruits = null;
		List<UploadFile>? attachments = null;
		if (questions.HasItems())
		{
			var types = new List<PostType>() { PostType.Question, PostType.Option };
			attachments = await _attachmentsRepository.FetchByTypesAsync(types);
			allTerms = (await _termsRepository.FetchAllAsync()).ToList();
		}

		var pageList = questions.GetPagedList(_mapper, allRecruits, attachments, allTerms);
		foreach (var item in pageList.ViewList)
		{
			item.Options = item.Options.OrderByDescending(o => o.Correct).ToList();
		}

		return Ok(pageList);
	}

}
