using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Helpers;
using Newtonsoft.Json;

namespace ApplicationCore.Services;

public interface IDbExportService
{
	void Exmport(string folderPath);
}

public class DbExmportService : IDbExportService
{
	private readonly DefaultContext _context;
	public DbExmportService(DefaultContext context)
	{
		_context = context;
	}

	public void Exmport(string folderPath)
	{
		_context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

		var subjects = _context.Subjects.ToList();
		SaveJson(folderPath, new Subject().GetType().Name, JsonConvert.SerializeObject(subjects));

		var terms = _context.Terms.ToList();
		SaveJson(folderPath, new Term().GetType().Name, JsonConvert.SerializeObject(terms));

		var questions = _context.Questions.ToList();
		SaveJson(folderPath, new Question().GetType().Name, JsonConvert.SerializeObject(questions));

		var options = _context.Options.ToList();
		SaveJson(folderPath, new Option().GetType().Name, JsonConvert.SerializeObject(options));

		var termQuestions = _context.TermQuestions.ToList();
		SaveJson(folderPath, new TermQuestion().GetType().Name, JsonConvert.SerializeObject(termQuestions));


		var resolves = _context.Resolves.ToList();
		SaveJson(folderPath, new Resolve().GetType().Name, JsonConvert.SerializeObject(resolves));

		var recruits = _context.Recruits.ToList();
		SaveJson(folderPath, new Recruit().GetType().Name, JsonConvert.SerializeObject(recruits));

		var recruitQuestions = _context.RecruitQuestions.ToList();
		SaveJson(folderPath, new RecruitQuestion().GetType().Name, JsonConvert.SerializeObject(recruitQuestions));

		var notes = _context.Notes.ToList();
		SaveJson(folderPath, new Note().GetType().Name, JsonConvert.SerializeObject(notes));

		var articles = _context.Articles.ToList();
		SaveJson(folderPath, new Article().GetType().Name, JsonConvert.SerializeObject(articles));

		var manuals = _context.Manuals.ToList();
		SaveJson(folderPath, new Manual().GetType().Name, JsonConvert.SerializeObject(manuals));

		var features = _context.Features.ToList();
		SaveJson(folderPath, new Feature().GetType().Name, JsonConvert.SerializeObject(features));

		var uploads = _context.UploadFiles.ToList();
		SaveJson(folderPath, new UploadFile().GetType().Name, JsonConvert.SerializeObject(uploads));

		var reviewRecords = _context.ReviewRecords.ToList();
		SaveJson(folderPath, new ReviewRecord().GetType().Name, JsonConvert.SerializeObject(reviewRecords));
	}
	void SaveJson(string folderPath, string name, string content)
	{
		var filePath = Path.Combine(folderPath, $"{name}.json");
		System.IO.File.WriteAllText(filePath, content);
	}
}
