using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers.Api;

[Authorize]
public class TermsController : BaseApiController
{
	private readonly IMapper _mapper;
	private readonly IDefaultRepository<Term> _termsRepository;

	public TermsController(IDefaultRepository<Term> termsRepository, IMapper mapper)
	{
		_mapper = mapper;
		_termsRepository = termsRepository;
	}

	[HttpGet("{id}")]
	public async Task<ActionResult> Details(int id)
	{
		var term = await _termsRepository.FindTermLoadSubItemsAsync(id);
		if (term == null) return NotFound();

		return Ok(term.MapViewModel(_mapper));
	}
}
