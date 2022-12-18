using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.DataAccess;
using Web.Models;
using ApplicationCore.Specifications;

namespace Web.Controllers.Admin;

public class PaysController : BaseAdminController
{
	private readonly IDefaultRepository<Pay> _paysRepository;
	private readonly IDefaultRepository<PayWay> _paywaysRepository;
	private readonly IDefaultRepository<Plan> _plansRepository;
	private readonly IDefaultRepository<Bill> _billsRepository;
	private readonly IMapper _mapper;

	public PaysController(IDefaultRepository<Pay> paysRepository, IDefaultRepository<PayWay> paywaysRepository,
		IDefaultRepository<Plan> plansRepository, IDefaultRepository<Bill> billsRepository, IMapper mapper)
	{
		_paysRepository = paysRepository;
		_paywaysRepository = paywaysRepository;
		_plansRepository = plansRepository;
		_billsRepository = billsRepository;
		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int plan = 0, string start = "", string end = "", int page = 1, int pageSize = 10)
	{
		var model = new PaysAdminModel();

		if (page < 0) //首次載入
		{
			page = 1;
			var payways = await _paywaysRepository.FetchAllAsync();
			payways = payways.GetOrdered();
			model.Payways = payways.MapViewModelList(_mapper);
		}

		IEnumerable<Pay> pays;
		if (plan > 0)
		{
			var planSelected = await _plansRepository.GetByIdAsync(plan);
			if (planSelected == null)
			{
				ModelState.AddModelError("plan", "方案不存在");
				return BadRequest(ModelState);
			}
			var bills = await _billsRepository.FetchByPlanAsync(planSelected);
			var billIds = bills.Select(x => x.Id).ToList();
			if (billIds.HasItems()) pays = await _paysRepository.FetchByBillsAsync(billIds);
			else pays = new List<Pay>();
		}
		else
		{
			pays = await _paysRepository.FetchAllAsync();
		}

		if (start.HasValue() || end.HasValue())
		{
			var startDate = start.ToStartDate();
			if (!startDate.HasValue) startDate = DateTime.MinValue;

			var endDate = end.ToEndDate();
			if (!endDate.HasValue) endDate = DateTime.MaxValue;


			pays = pays.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate);
		}



		pays = pays.GetOrdered();

		model.PagedList = pays.GetPagedList(_mapper, null, page, pageSize);
		return Ok(model);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult> Details(int id)
	{
		var pay = await _paysRepository.FirstOrDefaultAsync(new PaysSpecification(id));
		if (pay == null) return NotFound();

		var model = pay.MapViewModel(_mapper, null);

		return Ok(model);
	}

}
