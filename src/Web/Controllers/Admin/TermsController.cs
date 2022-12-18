using ApplicationCore.Helpers;
using ApplicationCore.Views;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Models;
using Web.Models.Admin;

namespace Web.Controllers.Admin;

public class TermsController : BaseAdminController
{
	private readonly IMapper _mapper;
	private readonly IDefaultRepository<Term> _termsRepository;
	private readonly IDefaultRepository<Subject> _subjectsRepository;

	public TermsController(IDefaultRepository<Term> termsRepository, IDefaultRepository<Subject> subjectsRepository, IMapper mapper)
	{
		_mapper = mapper;
		_termsRepository = termsRepository;
		_subjectsRepository = subjectsRepository;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int subject, int parent = -1, string keyword = "", bool subItems = true)
	{
		var selectedSubject = await _subjectsRepository.GetByIdAsync(subject);
		if (selectedSubject == null)
		{
			ModelState.AddModelError("subject", "科目不存在");
			return BadRequest(ModelState);
		}

		var terms = await _termsRepository.FetchAsync(selectedSubject, parent);

		if (terms.HasItems())
		{
			if (subItems) _termsRepository.LoadSubItems(terms);

			var keywords = keyword.GetKeywords();
			if (keywords.HasItems()) terms = terms.FilterByKeyword(keywords);

			terms = terms.GetOrdered();
		}


		return Ok(terms.MapViewModelList(_mapper));
	}




	[HttpGet("create")]
	public async Task<ActionResult> Create(int subject, int parent)
	{
		var selectedSubject = await _subjectsRepository.GetByIdAsync(subject);
		if (selectedSubject == null)
		{
			ModelState.AddModelError("subject", "科目不存在");
			return BadRequest(ModelState);
		}

		int maxOrder = await _termsRepository.GetMaxOrderAsync(selectedSubject, parent);
		int order = maxOrder + 1;
		var model = new TermViewModel()
		{
			Order = order,
			SubjectId = subject,
			ParentId = parent,
			Active = order >= 0
		};

		var terms = await _termsRepository.FetchAsync(selectedSubject);
		if (terms.HasItems())
		{
			_termsRepository.LoadSubItems(terms);
			terms = terms.GetOrdered();
		}

		var form = new TermEditForm()
		{
			Term = model,
			Parents = terms.MapViewModelList(_mapper)
		};
		return Ok(form);
	}

	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] TermViewModel model)
	{
		await ValidateRequestAsync(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var term = model.MapEntity(_mapper, CurrentUserId, null);

		term = await _termsRepository.AddAsync(term);

		return Ok(term.Id);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult> Details(int id)
	{
		var term = await _termsRepository.FindTermLoadSubItemsAsync(id);
		if (term == null) return NotFound();

		return Ok(term.MapViewModel(_mapper));
	}

	[HttpGet("edit/{id}")]
	public async Task<ActionResult> Edit(int id)
	{
		var term = await _termsRepository.GetByIdAsync(id);
		if (term == null) return NotFound();

		var model = term.MapViewModel(_mapper);
		model.Text = model.Text!.ReplaceBrToNewLine();


		var selectedSubject = await _subjectsRepository.GetByIdAsync(term.SubjectId);
		var terms = await _termsRepository.FetchAsync(selectedSubject!);
		if (terms.HasItems())
		{
			_termsRepository.LoadSubItems(terms);
			terms = terms.GetOrdered();
		}

		var form = new TermEditForm()
		{
			Term = model,
			Parents = terms.MapViewModelList(_mapper)
		};
		return Ok(form);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] TermViewModel model)
	{
		var term = await _termsRepository.FindTermLoadSubItemsAsync(id);
		if (term == null) return NotFound();

		await ValidateRequestAsync(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		term = model.MapEntity(_mapper, CurrentUserId, term);

		await _termsRepository.UpdateAsync(term);

		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		var term = await _termsRepository.GetByIdAsync(id);
		if (term == null) return NotFound();

		term.Removed = true;
		term.SetUpdated(CurrentUserId);
		await _termsRepository.UpdateAsync(term);

		return Ok();
	}

	async Task ValidateRequestAsync(TermViewModel model)
	{
		var subject = await _subjectsRepository.GetByIdAsync(model.SubjectId);
		if (subject == null) ModelState.AddModelError("subjectId", "科目不存在");

		if (model.ParentId > 0)
		{
			var parent = await _termsRepository.GetByIdAsync(model.ParentId);
			if (parent == null) ModelState.AddModelError("parentId", "主條文不存在");
			else 
			{
				if (parent.Id == model.Id) ModelState.AddModelError("parentId", "主條文重疊.請選擇其他主條文");
			}
			
		}
	}


}
