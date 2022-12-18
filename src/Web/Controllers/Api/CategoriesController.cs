using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Services;

namespace Web.Controllers.Api;

public class CategoriesController : BaseApiController
{
	private readonly IDataService _dataService;
	private readonly IMapper _mapper;

	public CategoriesController(IDataService dataService, IMapper mapper)
	{
		_dataService = dataService;
		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index()
	{
		var categories = await _dataService.FetchNoteCategoriesAsync();
		return Ok(categories);
	}

}


