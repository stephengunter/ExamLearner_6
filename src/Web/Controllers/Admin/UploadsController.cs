using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using ApplicationCore.Models;
using ApplicationCore.Views;
using SixLabors.ImageSharp;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using ApplicationCore.Specifications;

namespace Web.Controllers.Admin;

public class UploadsController : BaseAdminController
{
	private readonly IWebHostEnvironment _environment;
	private readonly AppSettings _appSettings;
	private readonly IDefaultRepository<UploadFile> _attachmentsRepository;
	private readonly IMapper _mapper;

	public UploadsController(IWebHostEnvironment environment, IOptions<AppSettings> appSettings,
		IDefaultRepository<UploadFile> attachmentsRepository, IMapper mapper)
	{
		_environment = environment;
		_appSettings = appSettings.Value;
		_attachmentsRepository = attachmentsRepository;
		_mapper = mapper;
	}

	string UploadFilesPath => Path.Combine(_environment.WebRootPath, _appSettings.UploadPath.HasValue() ? _appSettings.UploadPath : "uploads");

	[HttpGet("")]
	public async Task<IActionResult> Index(string type = "", int page = 1, int pageSize = 12)
	{
		if (String.IsNullOrEmpty(type))
		{
			var attachments = await _attachmentsRepository.ListAsync(new AttachmentsSpecification());
			if(attachments.HasItems()) attachments = attachments.OrderByDescending(x => x.CreatedAt).ToList();

			return Ok(attachments.GetPagedList(_mapper, page, pageSize));
		}


		try
		{
			var postType = type.ToEnum<PostType>();

			var attachments = await _attachmentsRepository.FetchByTypesAsync(new List<PostType> { postType });
			if (attachments.HasItems()) attachments = attachments.OrderByDescending(x => x.CreatedAt).ToList();

			return Ok(attachments.GetPagedList(_mapper, page, pageSize));

		}
		catch (Exception)
		{
			ModelState.AddModelError("type", "錯誤的 type");
			return BadRequest(ModelState);
		}


	}


	[HttpPost("")]
	public async Task<IActionResult> Store([FromForm] UploadForm form)
	{
		PostType postType = form.GetPostType();
		int postId = form.PostId;

		var attachments = new List<UploadFile>();

		foreach (var file in form.Files!)
		{
			if (file.Length > 0)
			{
				string fileName = file.FileName;
				var attachment = await GetUploadFileAsync(postType, postId, fileName);
				if (attachment == null) throw new Exception(String.Format("attachmentRepository.FindByName({0},{1})", file.FileName, form.PostId));

				string folder = postType == PostType.Emoji ? "emoji" : "";
				var upload = await SaveFile(file, folder);
				attachment.PostType = postType;
				attachment.Type = upload.Type;
				attachment.Path = upload.Path;

				switch (upload.Type)
				{
					case ".jpg":
					case ".jpeg":
					case ".png":
					case ".gif":
						var image = Image.Load(file.OpenReadStream());
						attachment.Width = image.Width;
						attachment.Height = image.Height;
						attachment.PreviewPath = upload.Path;
						break;
				}

				attachments.Add(attachment);
			}
		}

		var addItems = attachments.Where(a => a.Id < 1).ToList();
		var updateItems = attachments.Where(a => a.Id > 0).ToList();

		await _attachmentsRepository.AddRangeAsync(addItems);

		await _attachmentsRepository.UpdateRangeAsync(updateItems);

		return Ok(attachments);

	}

	async Task<UploadFile?> GetUploadFileAsync(PostType postType, int postId, string fileName)
	{
		if (postType == PostType.None || postId < 1) return new UploadFile();

		return await _attachmentsRepository.FindByNameAsync(fileName, postType, postId);
	}


	async Task<UploadFile> SaveFile(IFormFile file, string folder = "")
	{
		//檢查檔案路徑
		if (String.IsNullOrEmpty(folder)) folder = DateTime.Now.ToString("yyyyMMdd");

		string folderPath = Path.Combine(this.UploadFilesPath, folder);
		if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

		string extension = Path.GetExtension(file.FileName).ToLower();

		string fileName = String.Format("{0}{1}", Guid.NewGuid(), extension);
		string filePath = Path.Combine(folderPath, fileName);
		using (var fileStream = new FileStream(filePath, FileMode.Create))
		{
			await file.CopyToAsync(fileStream);
		}
		var entity = new UploadFile()
		{
			Type = extension,
			Path = folder + "/" + fileName
		};

		return entity;
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		var attachment = await _attachmentsRepository.GetByIdAsync(id);
		if (attachment == null) return NotFound();

		attachment.Removed = true;
		attachment.SetUpdated(CurrentUserId);
		await _attachmentsRepository.UpdateAsync(attachment);

		return Ok();
	}

}
