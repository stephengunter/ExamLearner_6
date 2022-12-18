using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;

namespace Web.Controllers.Api;

public class ManualsController : BaseApiController
{
	private readonly IDefaultRepository<Manual> _manualsRepository;
	private readonly IMapper _mapper;

	public ManualsController(IDefaultRepository<Manual> manualsRepository, IMapper mapper)
	{
		_manualsRepository = manualsRepository;
		_mapper = mapper;
	}


	[HttpGet("")]
	public async Task<ActionResult> Index()
	{
		var manuals = await _manualsRepository.FetchAllAsync();

		manuals = manuals.Where(x => x.Active);

		var rootItems = manuals.Where(x => x.ParentId == 0).GetOrdered();

		var subItems = manuals.Where(x => x.ParentId > 0).GetOrdered();

		foreach (var item in manuals) item.LoadSubItems(subItems);

		return Ok(rootItems.MapViewModelList(_mapper));
	}




}


