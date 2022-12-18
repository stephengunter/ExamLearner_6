using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Exceptions;
using Web.Services;

namespace Web.Controllers.Api;

[Authorize]
public class BillsController : BaseApiController
{
	private readonly ISubscribesService _subscribesService;

	public BillsController(ISubscribesService subscribesService)
	{
		_subscribesService = subscribesService;
	}

	[HttpGet("edit/{id}")]
	public async Task<ActionResult> Edit(int id)
	{
		var form = await _subscribesService.GetEditBillFormAsync(id, CurrentUserId);

		if (form == null) return NotFound();

		return Ok(form);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult> Details(int id)
	{
		var model = await _subscribesService.GetBillDetailsViewAsync(id, CurrentUserId);

		if (model == null) return NotFound();

		return Ok(model);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] BillViewModel model)
	{
		//begin pay,  請求ecpayTradeToken啟動付款程序
		try
		{
			var ecPayTradeModel = await _subscribesService.BeginPayAsync(id, model, CurrentUserId);

			if (ecPayTradeModel == null) return NotFound();

			return Ok(ecPayTradeModel);
		}
		catch (Exception ex)
		{
			if (ex is BadRequestException)
			{
				var badRequestException = ex as BadRequestException;
				foreach (var err in badRequestException!.Errors)
				{
					ModelState.AddModelError(err.Key, err.Message!);
				}
				return BadRequest(ModelState);
			}
			throw;
		}
	}

}


