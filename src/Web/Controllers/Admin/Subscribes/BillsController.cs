using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.DataAccess;

namespace Web.Controllers.Admin;

public class BillsController : BaseAdminController
{
	private readonly IDefaultRepository<Bill> _billsRepository;
	private readonly IDefaultRepository<Plan> _plansRepository;
	private readonly IMapper _mapper;

	public BillsController(IDefaultRepository<Bill> billsRepository, IDefaultRepository<Plan> plansRepository, IMapper mapper)
	{
		_billsRepository = billsRepository;
		_plansRepository = plansRepository;
		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int plan, int payed = 1, int page = 1, int pageSize = 10)
	{
		Plan? planSelected = null;
		if (plan > 0)
		{
			planSelected = await _plansRepository.GetByIdAsync(plan);
			if (planSelected == null)
			{
				ModelState.AddModelError("plan", "方案不存在");
				return BadRequest(ModelState);
			}
		}

		var bills = await _billsRepository.FetchAllAsync();

		if (planSelected != null) bills = bills.Where(x => x.PlanId == planSelected.Id);

		bills = bills.Where(x => x.Payed == payed.ToBoolean());

		bills = bills.GetOrdered();


		if (page < 1) page = 1;
		return Ok(bills.GetPagedList(_mapper, page, pageSize));
	}


	[HttpGet("{id}")]
	public async Task<ActionResult> Details(int id)
	{
		var bill = await _billsRepository.FindBillIncludeItemsAsync(id);
		if (bill == null) return NotFound();

		var model = bill.MapViewModel(_mapper, null);

		return Ok(model);
	}

	[HttpPut("clear/{plan}")]
	public async Task<ActionResult> Clear(int plan)
	{
		Plan? planSelected = null;
		if (plan > 0)
		{
			planSelected = await _plansRepository.GetByIdAsync(plan);
			if (planSelected == null)
			{
				ModelState.AddModelError("plan", "方案不存在");
				return BadRequest(ModelState);
			}
		}

		var bills = await _billsRepository.FetchAllAsync();
		if (planSelected != null) bills = bills.Where(x => x.PlanId == planSelected.Id);

		var targetBills = bills.Where(x => x.TotalPayed == 0).Where(x => x.DeadLine.HasValue && DateTime.Now > x.DeadLine);
		foreach (var target in targetBills)
		{
			await _billsRepository.RemoveAsync(target);
		} 

		return Ok();
	}

}
