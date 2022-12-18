using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.DataAccess;

namespace Web.Controllers.Admin;
public class SubscribesController : BaseAdminController
{
	private readonly IDefaultRepository<Subscribe> _subscribesRepository;
	private readonly IDefaultRepository<Plan> _plansRepository;
	private readonly IMapper _mapper;

	public SubscribesController(IDefaultRepository<Subscribe> subscribesRepository, IDefaultRepository<Plan> plansRepository, IMapper mapper)
	{
		_subscribesRepository = subscribesRepository;
		_plansRepository = plansRepository;
		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int plan, int page = 1, int pageSize = 10)
	{
		Plan? planSelected = null;
		if (plan > 0) planSelected = await _plansRepository.GetByIdAsync(plan);

		if (planSelected == null)
		{
			ModelState.AddModelError("plan", "方案不存在");
			return BadRequest(ModelState);
		}

		var subscribes = await _subscribesRepository.FetchByPlanAsync(planSelected);
		subscribes = subscribes.GetOrdered();

		if (page < 1) page = 1;
		return Ok(subscribes.GetPagedList(_mapper, page, pageSize));
	}

}
