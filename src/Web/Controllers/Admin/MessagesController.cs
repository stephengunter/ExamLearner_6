using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using AutoMapper;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using ApplicationCore.Settings;
using ApplicationCore.DataAccess;

namespace Web.Controllers.Admin;

public class MessagesController : BaseAdminController
{
	private readonly IWebHostEnvironment _environment;
	private readonly AppSettings _appSettings;

	private readonly IDefaultRepository<Message> _messagesRepository;
	private readonly IMailService _mailService;
	private readonly IMapper _mapper;

	public MessagesController(IWebHostEnvironment environment, IOptions<AppSettings> appSettings,
		IDefaultRepository<Message> messagesRepository, IMailService mailService, IMapper mapper)
	{
		_environment = environment;
		_appSettings = appSettings.Value;

		_messagesRepository = messagesRepository;
		_mailService = mailService;
		_mapper = mapper;
	}


	[HttpGet("")]
	public async Task<ActionResult> Index(int status = 0, string start = "", string end = "", int page = 1, int pageSize = 10)
	{
		if (page < 1) page = 1;
		var messages =  await _messagesRepository.FetchAsync(status.ToBoolean());

		if (messages.HasItems())
		{
			if (start.HasValue() || end.HasValue())
			{
				var startDate = start.ToStartDate();
				if (!startDate.HasValue) startDate = DateTime.MinValue;

				var endDate = end.ToEndDate();
				if (!endDate.HasValue) endDate = DateTime.MaxValue;

				messages = messages.Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate).ToList();
			}

			messages = messages.GetOrdered().ToList();
		}

		return Ok(messages.GetPagedList(_mapper, page, pageSize));
	}

	[HttpGet("edit/{id}")]
	public async Task<ActionResult> Edit(int id)
	{
		var message = await _messagesRepository.GetByIdAsync(id);
		if (message == null) return NotFound();

		var model = message.MapViewModel(_mapper);

		if (!message.Returned)
		{
			if (String.IsNullOrEmpty(model.ReturnContentView!.Template))
			{
				model.ReturnContentView.Template = GetMailTemplate(_environment, _appSettings);
			}

		}

		return Ok(model);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] BaseMessageViewModel model)
	{
		var message = await _messagesRepository.GetByIdAsync(id);
		if (message == null) return NotFound();


		if (!model.Draft)
		{
			ValidateRequest(model);
			if (!ModelState.IsValid) return BadRequest(ModelState);

			//發送email, 如果失敗則拋出例外
			await _mailService.SendAsync(message.Email!, model.Subject!, model.Content!, model.Text!);


			//如果發送成功, 狀態為已回覆
			message.Returned = true;

		}

		model.UpdatedBy = CurrentUserId;
		model.LastUpdated = DateTime.Now;

		message.ReturnContent = JsonConvert.SerializeObject(model);
		await _messagesRepository.UpdateAsync(message);

		return Ok();
	}

	void ValidateRequest(BaseMessageViewModel model)
	{
		if (String.IsNullOrEmpty(model.Subject)) ModelState.AddModelError("subject", "必須填寫主旨");

		if (String.IsNullOrEmpty(model.Content)) ModelState.AddModelError("content", "必須填寫內容");

	}


}
