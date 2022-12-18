using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Exceptions;
using ApplicationCore.Helpers;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;
using ApplicationCore.Hubs;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Hangfire;
using Web.Services;
using ApplicationCore.Services;
using ApplicationCore.DataAccess;

namespace Web.Controllers.Api;

[EnableCors("EcPay")]
public class PaysController : BaseApiController
{
	private readonly ISubscribesService _subscribesService;

	private readonly IMailService _mailService;
	private readonly IDefaultRepository<Notice> _noticesRepository;
	private readonly IHubContext<NotificationsHub> _notificationHubContext;
	private readonly IHubConnectionManager _userConnectionManager;

	private readonly IWebHostEnvironment _environment;
	private readonly AppSettings _appSettings;
	private readonly ILogger<PaysController> _logger;

	public PaysController(ISubscribesService subscribesService, IMailService mailService,
		IHubContext<NotificationsHub> notificationHubContext, IHubConnectionManager userConnectionManager,
		IDefaultRepository<Notice> noticesRepository, IWebHostEnvironment environment,
		 IOptions<AppSettings> appSettings, ILogger<PaysController> logger)
	{
		_subscribesService = subscribesService;
		_mailService = mailService;

		_notificationHubContext = notificationHubContext;
		_userConnectionManager = userConnectionManager;

		_noticesRepository = noticesRepository;
		_environment = environment;
		_appSettings = appSettings.Value;
		_logger = logger;
	}

	[HttpPost("")]
	public async Task<ActionResult> Store()
	{
		TradeResultModel? tradeResultModel = null;
		try
		{
			tradeResultModel = _subscribesService.ResolveTradeResult(Request);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.ToString());

			if (ex is EcPayTradeFeedBackFailed)
			{
				// rtnCode 不是 1 也不是 2
				_logger.LogInformation("ResolveTradeResult: 1|OK");
				return Ok("1|OK");
			}
			else if (ex is EcPayTradeFeedBackError)
			{
				_logger.LogInformation($"ResolveTradeResult: 0|{ex.Message}");
				return Ok($"0|{ex.Message}");
			}
			else
			{
				throw;
			}

		}

		var subscribe = await _subscribesService.StorePayAsync(tradeResultModel);
		if (subscribe != null)
		{
			//付款訂閱完成
			BackgroundJob.Enqueue(() => NotifyUserAsync(subscribe.UserId));
		}

		return Ok("1|OK");

	}

	[ApiExplorerSettings(IgnoreApi = true)]
	public async Task NotifyUserAsync(string userId)
	{

		try
		{
			var user = await _subscribesService.FindUserByIdAsync(userId);
			if (user == null) throw new UserNotFoundException(userId);

			//create private notice
			string content = GetMailTemplate(_environment, _appSettings, "subscribe");
			var notice = new Notice
			{
				Title = "您的訂閱會員已經生效",
				Content = content
			};
			await _noticesRepository.CreateUserNotificationAsync(notice, new List<string> { userId });

			// send email
			string mailSubject = notice.Title;
			string mailTemplate = GetMailTemplate(_environment, _appSettings);
			string mailContent = mailTemplate.Replace(Web.Consts.MAIL_CONTENT, content);

			await _mailService.SendAsync(user.Email, notice.Title, mailContent);


			// send hub notification
			var connections = _userConnectionManager.GetUserConnections(userId);
			if (connections.HasItems())
			{
				foreach (var connectionId in connections)
				{
					await _notificationHubContext.Clients.Client(connectionId).SendAsync(Web.Consts.Notifications, userId);
				}
			}
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, ex.ToString());
		}

	}

}


