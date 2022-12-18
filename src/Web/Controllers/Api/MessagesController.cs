using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using ApplicationCore.Services;
using Web.Models;

namespace Web.Controllers.Api;
public class MessagesController : BaseApiController
{
	private readonly IHttpContextAccessor _accessor;
	private readonly IRecaptchaService _recaptchaService;
	private readonly IDefaultRepository<Message> _messagesRepository;
	private readonly IMapper _mapper;

	public MessagesController(IHttpContextAccessor accessor, IRecaptchaService recaptchaService,
		IDefaultRepository<Message> messagesRepository, IMapper mapper)
	{
		_accessor = accessor;
		_recaptchaService = recaptchaService;
		_messagesRepository = messagesRepository;
		_mapper = mapper;
	}


	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] MessageEditForm form)
	{
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var ip = _accessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
		bool recaptchaValid = await _recaptchaService.VerifyAsync(form.Token, ip);

		if (!recaptchaValid)
		{
			ModelState.AddModelError("recaptcha", "驗證失敗");
			return BadRequest(ModelState);
		}

		var message = form.Message.MapEntity(_mapper, CurrentUserId);
		message = await _messagesRepository.AddAsync(message);

		return Ok(message.MapViewModel(_mapper));
	}

}
