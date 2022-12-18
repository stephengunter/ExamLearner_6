using ApplicationCore.Settings;
using ApplicationCore.Views;
using ApplicationCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[Route("[controller]")]
[ApiController]
public abstract class BaseController : Controller
{
	protected string RemoteIpAddress
	{
		get 
		{
			var ip = Request.HttpContext.Connection.RemoteIpAddress;
			if (ip == null) return string.Empty;
			return ip.ToString();
		}
	}

	protected string CurrentUserName => User.Claims.UserName();

	protected string CurrentUserId => User.Claims.UserId();

	protected IEnumerable<string> CurrentUseRoles => User.Claims.Roles();

	protected bool CurrentUserIsSubscriber => User.Claims.IsSubscriber();

	protected IActionResult RequestError(string key, string msg)
	{
		ModelState.AddModelError(key, msg);
		return BadRequest(ModelState);
	}

	protected string MailTemplatePath(IWebHostEnvironment environment, AppSettings appSettings)
		=> Path.Combine(environment.WebRootPath, String.IsNullOrEmpty(appSettings.TemplatePath) ? "templates" : appSettings.TemplatePath);


	protected string GetMailTemplate(IWebHostEnvironment environment, AppSettings appSettings, string name = "default")
	{
		var pathToFile = Path.Combine(MailTemplatePath(environment, appSettings), $"{name}.html");
		if (!System.IO.File.Exists(pathToFile)) throw new Exception("email template file not found: " + pathToFile);

		string body = "";
		using (StreamReader reader = System.IO.File.OpenText(pathToFile))
		{
			body = reader.ReadToEnd();
		}

		return body.Replace("APPNAME", appSettings.Title).Replace("APPURL", appSettings.ClientUrl);

	}
}

[Route("api/[controller]")]
public abstract class BaseApiController : BaseController
{

}

[Route("admin/[controller]")]
public class BaseAdminController : BaseController
{
	protected void ValidateRequest(AdminRequest model, AdminSettings adminSettings)
	{
		if (model.Key != adminSettings.Key) ModelState.AddModelError("key", "認證錯誤");

	}
}


[Route("tests/[controller]")]
public abstract class BaseTestController : BaseApiController
{

}
