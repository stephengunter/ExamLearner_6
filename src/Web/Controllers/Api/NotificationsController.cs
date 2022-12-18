using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Authorization;
using ApplicationCore.Views;

namespace Web.Controllers.Api;

[Authorize]
public class NotificationsController : BaseApiController
{
	private readonly IDefaultRepository<Receiver> _receiversRepository;
	private readonly IMapper _mapper;

	public NotificationsController(IDefaultRepository<Receiver> receiverRepository,
		IMapper mapper)
	{
		_receiversRepository = receiverRepository;
		_mapper = mapper;
	}


	[HttpGet("")]
	public async Task<ActionResult> Index(int page = 1, int pageSize = 99)
	{
		if (page < 1) return await NotificationsAsync();

		var notifications = await _receiversRepository.FetchByUserAsync(new User { Id = CurrentUserId });
		notifications = notifications.GetOrdered();

		return Ok(notifications.GetPagedList(_mapper, page, pageSize));
	}

	async Task<ActionResult> NotificationsAsync()
	{
		var notifications = await _receiversRepository.FetchByUserAsync(new User { Id = CurrentUserId });
		// 只要未讀的
		notifications = notifications.Where(item => !item.HasReceived);

		return Ok(notifications.GetPagedList(_mapper, 1, 50));
	}

	[HttpGet("{id}")]
	public async Task<ActionResult> Details(int id)
	{
		var notification = await _receiversRepository.GetUserNotificationByIdAsync(id);
		if (notification == null) return NotFound();

		if (notification.UserId != CurrentUserId) return NotFound();

		return Ok(notification.MapViewModel(_mapper));
	}

	[HttpPost]
	public async Task<ActionResult> Clear([FromBody] CommonRequestViewModel model)
	{

		var idList = model.Data!.SplitToIds();
		if (idList.HasItems())
		{
			await _receiversRepository.ClearUserNotificationsAsync(new User { Id = CurrentUserId }, idList);
		}

		return await NotificationsAsync();
	}

}


