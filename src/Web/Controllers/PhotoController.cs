using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using ApplicationCore.Settings;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Helpers;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Web.Controllers.Api;

public class PhotoController : BaseController
{
	private readonly IWebHostEnvironment _environment;
	private readonly AppSettings _appSettings;
	private readonly IDefaultRepository<UploadFile> _attachmentsRepository;

	public PhotoController(IWebHostEnvironment environment, IOptions<AppSettings> appSettings,
		IDefaultRepository<UploadFile> attachmentsRepository)
	{
		_environment = environment;
		_appSettings = appSettings.Value;
		_attachmentsRepository = attachmentsRepository;
	}

	string UploadFilesPath => Path.Combine(_environment.WebRootPath, _appSettings.UploadPath);

	string GetImgSourcePath(string filename) => Path.Combine(UploadFilesPath, filename);

	[HttpGet("")]
	public IActionResult Index(string name, string type, int width = 0, int height = 0)
	{
		string imgSourcePath = ValidateRequest(name);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		// 長寬數值不正確時, 回傳原圖
		if (width < 0 || height < 0) return SendOriginalImage(imgSourcePath);
		if (width == 0 && height == 0) return SendOriginalImage(imgSourcePath);

		if (width == 0) width = height;
		else if (height == 0) height = width;


		var resizeType = type.ToImageResizeType();
		if (resizeType == ImageResizeType.Unknown) resizeType = ImageResizeType.Scale;

		using (var inStream = new MemoryStream(System.IO.File.ReadAllBytes(imgSourcePath)))
		using (var outStream = new MemoryStream())
		using (var imgSource = Image.Load(inStream, out IImageFormat format))
		{
			if (resizeType == ImageResizeType.Scale)
			{
				using (Image copy = imgSource.Clone(x => x.Resize(width, height)))
				{
					copy.Save(outStream, new JpegEncoder());
					return this.File(outStream, "image/jpeg");
				}
			}
			else 
			{
				var options = new ResizeOptions
				{
					Mode = ResizeMode.Crop,
					Size = new Size(width, height)
				};
				using (Image copy = imgSource.Clone(x => x.Resize(options)))
				{
					copy.Save(outStream, new JpegEncoder());
					return this.File(outStream, "image/jpeg");
				}
			}
			
		}

	}

	[HttpGet("{id}")]
	public async Task<IActionResult> Index(int id, int width = 0, int height = 0, string type = "")
	{
		var entity = await _attachmentsRepository.GetByIdAsync(id);
		if (entity == null) return NotFound();

		return Index(entity.PreviewPath!, type, width, height);
	}


	// 傳回原始圖片
	private IActionResult SendOriginalImage(string imgSourcePath)
	{
		string type = "image/jpeg";

		string ext = Path.GetExtension(imgSourcePath).ToLower();
		if (ext == "png") type = "image/png";
		else if (ext == "gif") type = "image/gif";

		using (var image = System.IO.File.OpenRead(imgSourcePath))
		{
			return File(image, type);
		}
	}

	string ValidateRequest(string name)
	{
		string imgSourcePath = GetImgSourcePath(name);
		if (!System.IO.File.Exists(imgSourcePath))
		{
			ModelState.AddModelError("path", "圖片路徑無效");
			return "";
		}

		string extension = (Path.HasExtension(imgSourcePath)) ?
									  System.IO.Path.GetExtension(imgSourcePath).Substring(1).ToLower() :
									  string.Empty;


		if (!("jpg".Equals(extension) || "gif".Equals(extension) || "png".Equals(extension)))
		{
			ModelState.AddModelError("path", "圖片格式錯誤");
			return "";
		}

		return imgSourcePath;

	}

}
