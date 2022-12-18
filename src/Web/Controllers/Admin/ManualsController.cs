using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Helpers;
using Web.Models;
using Infrastructure.Views;

namespace Web.Controllers.Admin;

public class ManualsController : BaseAdminController
{
	private readonly IDefaultRepository<Manual> _manualsRepository;
	private readonly IMapper _mapper;

	public ManualsController(IDefaultRepository<Manual> manualsRepository,
		 IMapper mapper)
	{
		_manualsRepository = manualsRepository;
		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int active = 1)
	{
		var manuals = await _manualsRepository.FetchAsync(active.ToBoolean());
		foreach (var item in manuals)
		{
			if(item.Features!.HasItems()) item.Features = item.Features!.Where(x => !x.Removed).ToList();
		}
		manuals = manuals.GetOrdered();

		return Ok(manuals.MapViewModelList(_mapper));
	}

	[HttpGet("create")]
	public async Task<ActionResult> Create()
	{
		int parentId = 0;
		var rootItems = await _manualsRepository.FetchAsync(parentId);

		var parentsOptions = rootItems.Select(item => new BaseOption<int>(item.Id, item.Title)).ToList();
		var form = new ManualEditForm(parentsOptions, new ManualViewModel() { Order = -1 });

		return Ok(form);
	}

	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] ManualViewModel model)
	{
		ValidateRequest(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		model.Order = model.Active ? 0 : -1;

		var manual = model.MapEntity(_mapper, CurrentUserId);

		manual = await _manualsRepository.AddAsync(manual);

		return Ok(manual.Id);
	}

	[HttpGet("edit/{id}")]
	public async Task<ActionResult> Edit(int id)
	{
		var manual = await _manualsRepository.GetByIdAsync(id);
		if (manual == null) return NotFound();

		int parentId = 0;
		var rootItems = await _manualsRepository.FetchAsync(parentId);

		var parentsOptions = rootItems.Select(item => new BaseOption<int>(item.Id, item.Title)).ToList();
		var form = new ManualEditForm(parentsOptions, manual.MapViewModel(_mapper));

		return Ok(form);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] ManualViewModel model)
	{
		var manual = await _manualsRepository.GetByIdAsync(id);
		if (manual == null) return NotFound();

		ValidateRequest(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		manual = model.MapEntity(_mapper, CurrentUserId, manual);
		manual.Order = model.Active ? 0 : -1;

		await _manualsRepository.UpdateAsync(manual);

		return Ok();
	}


	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		var manual = await _manualsRepository.GetByIdAsync(id);
		if (manual == null) return NotFound();

		manual.Removed = true;
		manual.SetUpdated(CurrentUserId);

		await _manualsRepository.UpdateAsync(manual);

		return Ok();
	}

	void ValidateRequest(ManualViewModel model)
	{
		if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "請填寫標題");
	}
}
