using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;
using ApplicationCore.Services;
using ApplicationCore.Views;

namespace Web.Controllers.Admin;

public class PlansController : BaseAdminController
{
	private readonly IDefaultRepository<Plan> _plansRepository;
	private readonly IDefaultRepository<Subscribe> _subscribesRepository;
	private readonly IUsersService _usersService;
	private readonly IMapper _mapper;

	public PlansController(IDefaultRepository<Plan> plansRepository, IDefaultRepository<Subscribe> subscribesRepository,
		IUsersService usersService, IMapper mapper)
	{
		_plansRepository = plansRepository;
		_subscribesRepository = subscribesRepository;
		_usersService = usersService;
		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int active = -1)
	{
		if (active < 0)
		{
			var allPlans = await _plansRepository.FetchAllAsync();
			allPlans = allPlans.GetOrdered();
			return Ok(allPlans.MapViewModelList(_mapper));
		}
		else
		{
			bool isAcive = active.ToBoolean();
			var plans = await _plansRepository.FetchAsync(isAcive);
			plans = plans!.GetOrdered();
			return Ok(plans.MapViewModelList(_mapper));
		}

	}

	[HttpGet("create")]
	public ActionResult Create()
	{
		var model = new PlanViewModel
		{
			Price = 360,
			Discount = 50,
			Description = "<ul><li>第二次訂閱者半價優惠</li></ul>"
		};
		return Ok(model);
	}

	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] PlanViewModel model)
	{
		var plan = model.MapEntity(_mapper, CurrentUserId);
		plan.Discount = 50;   //固定五折

		await ValidateAsync(plan);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		plan = await _plansRepository.AddAsync(plan);

		return Ok(plan.Id);
	}

	[HttpGet("edit/{id}")]
	public async Task<ActionResult> Edit(int id)
	{
		var plan = await _plansRepository.GetByIdAsync(id);
		if (plan == null) return NotFound();

		var model = plan.MapViewModel(_mapper);
		return Ok(model);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] PlanViewModel model)
	{
		var plan = await _plansRepository.GetByIdAsync(id);
		if (plan == null) return NotFound();

		plan = model.MapEntity(_mapper, CurrentUserId, plan);
		plan.Discount = 50;   //固定五折

		await ValidateAsync(plan);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		await _plansRepository.UpdateAsync(plan);

		var subscribes = await _subscribesRepository.FetchByPlanAsync(plan);
		if (subscribes.HasItems())
		{
			foreach (var subscribe in subscribes) subscribe.EndDate = plan.EndDate;

			await _subscribesRepository.UpdateRangeAsync(subscribes);
		}

		return Ok();
	}

	[HttpPut("clear/{id}")]
	public async Task<ActionResult> Clear(int id)
	{
		var plan = await _plansRepository.GetByIdAsync(id);
		if (plan == null) return NotFound();

		if (!plan.CanClear)
		{
			ModelState.AddModelError("canClear", "此方案無法結算");
			return BadRequest(ModelState);
		}

		var subscribes = await _subscribesRepository.FetchByPlanAsync(plan);
		if (subscribes.HasItems())
		{
			var userIds = subscribes.Select(x => x.UserId);
			foreach (var userId in userIds)
			{
				await _usersService.RemoveSubscriberRoleAsync(userId);
			}
		}

		plan.ClearDate = DateTime.Now;
		await _plansRepository.UpdateAsync(plan);

		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		var plan = await _plansRepository.GetByIdAsync(id);
		if (plan == null) return NotFound();

		if (plan.Active)
		{
			ModelState.AddModelError("endDate", "方案上架中無法刪除");
			return BadRequest(ModelState);
		}

		plan.Removed = true;
		plan.SetUpdated(CurrentUserId);
		await _plansRepository.UpdateAsync(plan);

		return Ok();
	}

	async Task ValidateAsync(Plan plan)
	{
		if (!plan.StartDate.HasValue)
		{
			ModelState.AddModelError("startDate", "必須填寫開始日期");
		}
		else
		{
			if (plan.EndDate.HasValue)
			{
				if (plan.EndDate <= plan.StartDate)
				{
					ModelState.AddModelError("endDate", "結束日期錯誤");
				}
				else
				{
					var existingPlans = (await _plansRepository.FetchAllAsync()).Where(x => x.Id != plan.Id).ToList();
					var hasDateConflict = existingPlans.Where(x => x.HasDateConflict(plan));

					if (hasDateConflict.HasItems())
					{
						ModelState.AddModelError("endDate", "日期與其他方案衝突");
					}
				}
			}

		}
		if (plan.Money < 150 || plan.Discount > 500)
		{
			ModelState.AddModelError("money", "金額錯誤");
		}
		if (plan.Discount < 50 || plan.Discount > 95)
		{
			ModelState.AddModelError("discount", "折扣錯誤");
		}

	}

}
