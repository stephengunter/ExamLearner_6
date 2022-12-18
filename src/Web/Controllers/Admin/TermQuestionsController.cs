using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Models;

namespace Web.Controllers.Admin;

public class TermQuestionsController : BaseAdminController
{
	private readonly IDefaultRepository<Question> _questionsRepository;
	private readonly IDefaultRepository<UploadFile> _attachmentsRepository;
	private readonly IDefaultRepository<Recruit> _recruitsRepository;
	private readonly IDefaultRepository<Subject> _subjectsRepository;
	private readonly IDefaultRepository<Term> _termsRepository;
	private readonly IMapper _mapper;

	public TermQuestionsController(IDefaultRepository<Question> questionsRepository, IDefaultRepository<Recruit> recruitsRepository, IDefaultRepository<UploadFile> attachmentsRepository,
		IDefaultRepository<Subject> subjectsRepository, IDefaultRepository<Term> termsRepository, IMapper mapper)
	{
		_questionsRepository = questionsRepository;
		_recruitsRepository = recruitsRepository;
		_attachmentsRepository = attachmentsRepository;
		_subjectsRepository = subjectsRepository;
		_termsRepository = termsRepository;

		_mapper = mapper;
	}


	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] TermQuestion model)
	{
		var term = await ValidateRequestAsync(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var qids = term!.QIds!.SplitToIds();
		if (!qids.Contains(model.QuestionId)) qids.Add(model.QuestionId);

		term.QIds = qids.JoinToStringIntegers();

		await _termsRepository.UpdateAsync(term);

		return Ok();
	}


	[HttpPost("remove")]
	public async Task<ActionResult> Remove([FromBody] TermQuestion model)
	{
		var term = await ValidateRequestAsync(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var qids = term!.QIds!.SplitToIds();
		if (!qids.Contains(model.QuestionId)) return Ok();

		qids.RemoveAll(item => item == model.QuestionId);

		term.QIds = qids.JoinToStringIntegers();

		await _termsRepository.UpdateAsync(term);

		return Ok();
	}

	async Task<Term?> ValidateRequestAsync(TermQuestion model)
	{
		var term = await _termsRepository.GetByIdAsync(model.TermId);
		if (term == null) ModelState.AddModelError("termId", "條文不存在");

		var question = await _questionsRepository.GetByIdAsync(model.QuestionId);
		if (question == null) ModelState.AddModelError("questionId", "試題不存在");

		return term;

	}


}
