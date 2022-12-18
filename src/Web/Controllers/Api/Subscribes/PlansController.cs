using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using Web.Services;

namespace Web.Controllers.Api;

public class PlansController : BaseApiController
{
	private readonly ISubscribesService _subscribesService;
	private readonly IMapper _mapper;

	public PlansController(ISubscribesService subscribesService, IMapper mapper)
	{
		_subscribesService = subscribesService;
		_mapper = mapper;
	}


	[HttpGet("")]
	public async Task<ActionResult> Index()
	{
		var plan = await _subscribesService.FindActivePlanAsync();
		if (plan == null) return Ok();

		return Ok(plan.MapViewModel(_mapper));

	}

}


