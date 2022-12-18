using ApplicationCore.Helpers;
using ApplicationCore.Views;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Models;

namespace Web.Controllers.Admin;

public class SubjectsController : BaseAdminController
{
	private readonly IMapper _mapper;
	private readonly IDefaultRepository<Subject> _subjectsRepository;

	public SubjectsController(IDefaultRepository<Subject> subjectsRepository, IMapper mapper)
	{
		_mapper = mapper;
		_subjectsRepository = subjectsRepository;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int parent = -1, bool subItems = true)
	{
		var subjects = await _subjectsRepository.FetchAsync(parent);

		subjects = subjects.GetOrdered();

		if (subItems)
		{
			_subjectsRepository.LoadSubItems(subjects);

			foreach (var item in subjects) item.GetSubIds();
		}

		return Ok(subjects.MapViewModelList(_mapper));
	}

	[HttpGet("create")]
	public ActionResult Create() => Ok(new SubjectViewModel() { Order = -1 });


	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] SubjectViewModel model)
	{
		await ValidateRequestAsync(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var subject = model.MapEntity(_mapper, CurrentUserId);

		subject = await _subjectsRepository.AddAsync(subject);

		return Ok(subject.Id);
	}

	[HttpGet("edit/{id}")]
	public async Task<ActionResult> Edit(int id)
	{
		var subject = await _subjectsRepository.GetByIdAsync(id);
		if (subject == null) return NotFound();

		return Ok(subject.MapViewModel(_mapper));
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] SubjectViewModel model)
	{
		var subject = await _subjectsRepository.FindSubjectLoadSubItemsAsync(id);
		if (subject == null) return NotFound();

		await ValidateRequestAsync(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		subject = model.MapEntity(_mapper, CurrentUserId, subject);

		await _subjectsRepository.UpdateAsync(subject);

		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		var subject = await _subjectsRepository.GetByIdAsync(id);
		if (subject == null) return NotFound();

		subject.Removed = true;
		subject.SetUpdated(CurrentUserId);
		await _subjectsRepository.UpdateAsync(subject);

		return Ok();
	}

	async Task ValidateRequestAsync(SubjectViewModel model)
	{
		if (model.ParentId > 0)
		{
			var parent = await _subjectsRepository.GetByIdAsync(model.ParentId);
			if (parent == null) ModelState.AddModelError("parentId", "主科目不存在");
			else
			{
				if (parent.Id == model.Id) ModelState.AddModelError("parentId", "主科目重疊.請選擇其他主科目");
			} 

		}
	}


}
