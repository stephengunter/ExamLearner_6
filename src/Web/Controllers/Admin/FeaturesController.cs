using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using AutoMapper;
using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;

namespace Web.Controllers.Admin;

public class FeaturesController : BaseAdminController
{
	private readonly IDefaultRepository<Feature> _featureRepository;
	private readonly IMapper _mapper;
	public FeaturesController(IDefaultRepository<Feature> featureRepository, IMapper mapper)
	{
		_featureRepository = featureRepository;
		_mapper = mapper;
	}

	[HttpGet("create")]
	public ActionResult Create()
	{
		return Ok(new FeatureViewModel() { Order = -1 });
	}

	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] FeatureViewModel model)
	{
		ValidateRequest(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		model.Order = model.Active ? 0 : -1;

		var feature = model.MapEntity(_mapper, CurrentUserId);

		feature = await _featureRepository.AddAsync(feature);

		return Ok(feature.Id);
	}

	[HttpGet("edit/{id}")]
	public async Task<ActionResult> Edit(int id)
	{
		var feature = await _featureRepository.FirstOrDefaultAsync(new FeaturesSpecification(id));
		if (feature == null) return NotFound();

		var model = feature.MapViewModel(_mapper);
		return Ok(model);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] FeatureViewModel model)
	{
		var feature = await _featureRepository.GetByIdAsync(id);
		if (feature == null) return NotFound();

		ValidateRequest(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		feature = model.MapEntity(_mapper, CurrentUserId, feature);
		feature.Order = model.Active ? 0 : -1;

		await _featureRepository.UpdateAsync(feature);

		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		var feature = await _featureRepository.GetByIdAsync(id);
		if (feature == null) return NotFound();
		
		feature.Removed = true;
		feature.SetUpdated(CurrentUserId);

		await _featureRepository.UpdateAsync(feature);

		return Ok();
	}

	void ValidateRequest(FeatureViewModel model)
	{
		if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "請填寫標題");
	}
}
