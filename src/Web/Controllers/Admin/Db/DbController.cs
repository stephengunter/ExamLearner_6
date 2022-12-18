using ApplicationCore.Models;
using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using ApplicationCore.Settings;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;
using System.Data;

namespace Web.Controllers.Admin;
public class DbController : BaseAdminController
{
	private readonly AdminSettings _adminSettings;
	private readonly IDbService _dBService;
	private readonly IDbExportService _dBExportService;
	private readonly IDbImportService _dBImportService;

	private readonly string _backupFolder;

	public DbController(IOptions<AdminSettings> adminSettings, IDbService dBService,
		IDbExportService dBExportService, IDbImportService dBImportService)
	{
		_adminSettings = adminSettings.Value;
		_dBService = dBService;
		_dBExportService = dBExportService;
		_dBImportService = dBImportService;

		var path = Path.Combine(_adminSettings.BackupPath, DateTime.Today.ToDateNumber().ToString());
		if (!Directory.Exists(path)) Directory.CreateDirectory(path);

		_backupFolder = path;
	}

	async Task<string> ReadFileTextAsync(IFormFile file)
	{
		var result = new StringBuilder();
		using (var reader = new StreamReader(file.OpenReadStream()))
		{
			while (reader.Peek() >= 0) result.AppendLine(await reader.ReadLineAsync());
		}
		return result.ToString();

	}

	[HttpGet("dbname")]
	public ActionResult DBName() => Ok(_dBService.GetDbName());

	[HttpPost("migrate")]
	public ActionResult Migrate([FromBody] AdminRequest model)
	{

		ValidateRequest(model, _adminSettings);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		_dBService.Migrate();

		return Ok();
	}

	[HttpPost("backup")]
	public ActionResult Backup([FromBody] AdminRequest model)
	{
		ValidateRequest(model, _adminSettings);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		string dbName = _dBService.GetDbName();
		string fileName = Path.Combine(_backupFolder, $"{dbName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.bak");

		_dBService.Backup(fileName);

		return Ok();
	}

	[HttpPost("export")]
	public ActionResult Export([FromBody] AdminRequest model)
	{
		ValidateRequest(model, _adminSettings);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		_dBExportService.Exmport(_backupFolder);

		return Ok();
	}

	[HttpPost("import")]
	public async Task<IActionResult> Import([FromForm] AdminFileRequest model)
	{
		ValidateRequest(model, _adminSettings);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var fileNames = new List<string>();

		if (model.Files.Count < 1)
		{
			ModelState.AddModelError("files", "必須上傳檔案");
			return BadRequest(ModelState);
		}

		var extensions = model.Files.Select(item => Path.GetExtension(item.FileName).ToLower());
		if (extensions.Any(x => x != ".json"))
		{
			ModelState.AddModelError("files", "檔案格式錯誤");
			return BadRequest(ModelState);
		}

		string content = "";
		string fileName = new Subject().GetType().Name;
		var file = model.GetFile(fileName);
		if (file != null)
		{
			fileNames.Add(fileName);
			content = await ReadFileTextAsync(file);
			var subjectModels = JsonConvert.DeserializeObject<List<Subject>>(content);
			_dBImportService.ImportSubjects(subjectModels!);

			_dBImportService.SyncSubjects(subjectModels!);

		}

		fileName = new Term().GetType().Name;
		file = model.GetFile(fileName);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var termModels = JsonConvert.DeserializeObject<List<Term>>(content);
			_dBImportService.ImportTerms(termModels!);

			_dBImportService.SyncTerms(termModels!);
		}



		file = model.GetFile(new Question().GetType().Name);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var questionModels = JsonConvert.DeserializeObject<List<Question>>(content);
			_dBImportService.ImportQuestions(questionModels!);

			_dBImportService.SyncQuestions(questionModels!);
		}

		file = model.GetFile(new Option().GetType().Name);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var optionModels = JsonConvert.DeserializeObject<List<Option>>(content);
			_dBImportService.ImportOptions(optionModels!);

			_dBImportService.SyncOptions(optionModels!);
		}

		file = model.GetFile(new TermQuestion().GetType().Name);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var termQuestionModels = JsonConvert.DeserializeObject<List<TermQuestion>>(content);
			_dBImportService.ImportTermQuestions(termQuestionModels!);

			_dBImportService.SyncTermQuestions(termQuestionModels!);
		}

		file = model.GetFile(new Resolve().GetType().Name);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var resolveModels = JsonConvert.DeserializeObject<List<Resolve>>(content);
			_dBImportService.ImportResolves(resolveModels!);

			_dBImportService.SyncResolves(resolveModels!);
		}

		file = model.GetFile(new Recruit().GetType().Name);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var recruitModels = JsonConvert.DeserializeObject<List<Recruit>>(content);
			_dBImportService.ImportRecruits(recruitModels!);

			_dBImportService.SyncRecruits(recruitModels!);
		}

		file = model.GetFile(new RecruitQuestion().GetType().Name);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var recruitQuestionModels = JsonConvert.DeserializeObject<List<RecruitQuestion>>(content);
			_dBImportService.ImportRecruitQuestions(recruitQuestionModels!);

			_dBImportService.SyncRecruitQuestions(recruitQuestionModels!);
		}

		file = model.GetFile(new Note().GetType().Name);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var noteModels = JsonConvert.DeserializeObject<List<Note>>(content);
			_dBImportService.ImportNotes(noteModels!);

			_dBImportService.SyncNotes(noteModels!);
		}

		file = model.GetFile(new Article().GetType().Name);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var articleModels = JsonConvert.DeserializeObject<List<Article>>(content);
			_dBImportService.ImportArticles(articleModels!);

			_dBImportService.SyncArticles(articleModels!);
		}

		file = model.GetFile(new Manual().GetType().Name);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var manualModels = JsonConvert.DeserializeObject<List<Manual>>(content);
			_dBImportService.ImportManuals(manualModels!);

			_dBImportService.SyncManuals(manualModels!);
		}

		file = model.GetFile(new Feature().GetType().Name);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var featureModels = JsonConvert.DeserializeObject<List<Feature>>(content);
			_dBImportService.ImportFeatures(featureModels!);

			_dBImportService.SyncFeatures(featureModels!);
		}


		file = model.GetFile(new UploadFile().GetType().Name);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var uploadFileModels = JsonConvert.DeserializeObject<List<UploadFile>>(content);
			_dBImportService.ImportUploadFiles(uploadFileModels!);

			_dBImportService.SyncUploadFiles(uploadFileModels!);
		}

		file = model.GetFile(new ReviewRecord().GetType().Name);
		if (file != null)
		{
			content = await ReadFileTextAsync(file);
			var reviewRecordModels = JsonConvert.DeserializeObject<List<ReviewRecord>>(content);
			_dBImportService.ImportReviewRecords(reviewRecordModels!);

			_dBImportService.SyncReviewRecords(reviewRecordModels!);
		}


		//end of import

		return Ok();
	}
}
