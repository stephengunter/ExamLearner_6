using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;

namespace Web.Controllers.Admin;

public class CloudsController : BaseAdminController
{
	private readonly AdminSettings _adminSettings;
	private readonly ICloudStorageService _cloudStorageService;

	public CloudsController(IOptions<AdminSettings> adminSettings, ICloudStorageService cloudStorageService)
	{
		_adminSettings = adminSettings.Value;
		_cloudStorageService = cloudStorageService;
	}

	#region Properties
	string _backupFolder = "";
	string BackupFolder
	{
		get
		{
			if (String.IsNullOrEmpty(_backupFolder))
			{
				var path = Path.Combine(_adminSettings.BackupPath, DateTime.Today.ToDateNumber().ToString());
				if (!Directory.Exists(path)) Directory.CreateDirectory(path);

				_backupFolder = path;
			}
			return _backupFolder;
		}
	}
	#endregion
	[HttpPost("")]
	public async Task<ActionResult> DbBackups([FromBody] AdminRequest model)
	{
		ValidateRequest(model, _adminSettings);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		string storageFolder = DateTime.Today.ToDateNumber().ToString();

		foreach (var filePath in Directory.GetFiles(BackupFolder))
		{
			var fileInfo = new FileInfo(filePath);
			await _cloudStorageService.UploadFileAsync(filePath, $"{storageFolder}/{fileInfo.Name}");
		}

		return Ok();
	}


}
