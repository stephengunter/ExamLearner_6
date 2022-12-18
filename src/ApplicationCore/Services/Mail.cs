using ApplicationCore.Exceptions;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using ApplicationCore.Helpers;
using System.Net;

namespace ApplicationCore.Services;

public interface IMailService
{
	Task SendAsync(string email, string subject, string htmlContent, string textContent = "");
}

public class SendGridService : IMailService
{
	private readonly string _apiKey;
	private readonly AppSettings _appSettings;

	public SendGridService(IOptions<AppSettings> appSettings)
	{
		_apiKey = Environment.GetEnvironmentVariable("SENDGRID_KEY")!;
		if (String.IsNullOrEmpty(_apiKey)) throw new EnvironmentVariableNotFound("SENDGRID_KEY");

		_appSettings = appSettings.Value;
	}

	bool IsSuccess(HttpStatusCode statusCode)
	{
		var status = (int)statusCode;
		return status >= 200 && status < 300;
	}

	EmailAddress From => new EmailAddress(_appSettings.Email, _appSettings.Title);

	public async Task SendAsync(string email, string subject, string htmlContent, string textContent = "")
	{
		var client = new SendGridClient(_apiKey);
		var to = new EmailAddress(email);
		var plainTextContent = textContent.HasValue() ? textContent : htmlContent;

		var msg = MailHelper.CreateSingleEmail(From, to, subject, plainTextContent, htmlContent);

		await ExecuteAsync(client, msg);
	}

	async Task ExecuteAsync(SendGridClient client, SendGridMessage msg)
	{
		var response = await client.SendEmailAsync(msg);

		if (!IsSuccess(response.StatusCode))
		{
			var body = await response.Body.ReadAsStringAsync();
			throw new EmailSendFailed($"status: {(int)response.StatusCode} {response.StatusCode.ToString()} , body: {body}");
		}
	}

}
