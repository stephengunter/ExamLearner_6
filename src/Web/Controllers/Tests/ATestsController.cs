using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Settings;
using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using Microsoft.Extensions.Options;
using ApplicationCore.Models;
using ApplicationCore.Views;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace Web.Controllers.Tests;

public class ATestsController : BaseTestController
{
	private readonly AdminSettings _adminSettings;
	private readonly DefaultContext _context;
	private readonly IMapper _mapper;
	public ATestsController(IOptions<AdminSettings> adminSettings, DefaultContext context, IMapper mapper)
	{
		_adminSettings = adminSettings!.Value;
		_context = context;
		_mapper = mapper;
	}
	[HttpGet("")]
	public async Task<ActionResult> Index()
	{
		
		return Ok();

	}
	
}
