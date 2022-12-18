using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Helpers;

namespace ApplicationCore.Services;

public interface IDbImportService
{
	void ImportQuestions(List<Question> models);
	void ImportSubjects(List<Subject> models);
	void ImportTerms(List<Term> models);
	void ImportOptions(List<Option> models);
	void ImportTermQuestions(List<TermQuestion> models);
	void ImportResolves(List<Resolve> models);
	void ImportRecruits(List<Recruit> models);
	void ImportRecruitQuestions(List<RecruitQuestion> models);
	void ImportNotes(List<Note> models);
	void ImportArticles(List<Article> articles);
	void ImportManuals(List<Manual> models);
	void ImportFeatures(List<Feature> models);
	void ImportUploadFiles(List<UploadFile> models);
	void ImportReviewRecords(List<ReviewRecord> models);


	void SyncSubjects(List<Subject> models);
	void SyncTerms(List<Term> models);
	void SyncQuestions(List<Question> models);
	void SyncOptions(List<Option> models);
	void SyncTermQuestions(List<TermQuestion> models);
	void SyncResolves(List<Resolve> models);
	void SyncRecruits(List<Recruit> models);
	void SyncRecruitQuestions(List<RecruitQuestion> models);
	void SyncNotes(List<Note> models);
	void SyncArticles(List<Article> models);
	void SyncManuals(List<Manual> models);
	void SyncFeatures(List<Feature> models);
	void SyncUploadFiles(List<UploadFile> models);
	void SyncReviewRecords(List<ReviewRecord> models);
}

public class DbImportService : IDbImportService
{
	private readonly DefaultContext _context;
	private readonly string _connectionString;
	public DbImportService(DefaultContext context)
	{
		_context = context;
		_connectionString = context.Database.GetDbConnection().ConnectionString;
	}
	public void ImportTerms(List<Term> models)
	{
		var newTerms = new List<Term>();
		foreach (var termModel in models)
		{
			var existingEntity = _context.Terms.Find(termModel.Id);
			if (existingEntity == null) newTerms.Add(termModel);
			else Update(existingEntity, termModel);
		}

		_context.SaveChanges();

		using (var context = new DefaultContext(_connectionString))
		{
			context.Terms.AddRange(newTerms);

			context.Database.OpenConnection();
			try
			{
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Terms ON");
				context.SaveChanges();
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Terms OFF");
			}
			finally
			{
				context.Database.CloseConnection();
			}
		}

	}
	public void ImportSubjects(List<Subject> models)
	{
		var newSubjects = new List<Subject>();
		foreach (var subjectModel in models)
		{
			var existingEntity = _context.Subjects.Find(subjectModel.Id);
			if (existingEntity == null) newSubjects.Add(subjectModel);
			else Update(existingEntity, subjectModel);
		}

		_context.SaveChanges();

		using (var context = new DefaultContext(_connectionString))
		{
			context.Subjects.AddRange(newSubjects);

			context.Database.OpenConnection();
			try
			{
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Subjects ON");
				context.SaveChanges();
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Subjects OFF");
			}
			finally
			{
				context.Database.CloseConnection();
			}
		}

	}

	public void ImportQuestions(List<Question> models)
	{
		var newQuestions = new List<Question>();
		foreach (var questionModel in models)
		{
			var existingEntity = _context.Questions.Find(questionModel.Id);
			if (existingEntity == null) newQuestions.Add(questionModel);
			else Update(existingEntity, questionModel);
		}

		_context.SaveChanges();

		using (var context = new DefaultContext(_connectionString))
		{
			context.Questions.AddRange(newQuestions);

			context.Database.OpenConnection();
			try
			{
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Questions ON");
				context.SaveChanges();
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Questions OFF");
			}
			finally
			{
				context.Database.CloseConnection();
			}
		}

	}

	public void ImportOptions(List<Option> models)
	{
		var newOptions = new List<Option>();
		foreach (var optionModel in models)
		{
			var existingEntity = _context.Options.Find(optionModel.Id);
			if (existingEntity == null) newOptions.Add(optionModel);
			else Update(existingEntity, optionModel);
		}

		_context.SaveChanges();

		using (var context = new DefaultContext(_connectionString))
		{
			context.Options.AddRange(newOptions);

			context.Database.OpenConnection();
			try
			{
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Options ON");
				context.SaveChanges();
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Options OFF");
			}
			finally
			{
				context.Database.CloseConnection();
			}
		}

	}

	public void ImportTermQuestions(List<TermQuestion> models)
	{
		var newTermQuestions = new List<TermQuestion>();
		foreach (var termQuestionModel in models)
		{
			var existingEntity = _context.TermQuestions.FirstOrDefault(x => x.TermId == termQuestionModel.TermId && x.QuestionId == termQuestionModel.QuestionId);
			if (existingEntity == null) newTermQuestions.Add(termQuestionModel);

		}
		_context.TermQuestions.AddRange(newTermQuestions);
		_context.SaveChanges();

	}

	public void ImportResolves(List<Resolve> models)
	{
		var newResolves = new List<Resolve>();
		foreach (var resolveModel in models)
		{
			var existingEntity = _context.Resolves.Find(resolveModel.Id);
			if (existingEntity == null) newResolves.Add(resolveModel);
			else Update(existingEntity, resolveModel);
		}

		_context.SaveChanges();

		using (var context = new DefaultContext(_connectionString))
		{
			context.Resolves.AddRange(newResolves);

			context.Database.OpenConnection();
			try
			{
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Resolves ON");
				context.SaveChanges();
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Resolves OFF");
			}
			finally
			{
				context.Database.CloseConnection();
			}
		}

	}

	public void ImportRecruits(List<Recruit> models)
	{
		var newRecruits = new List<Recruit>();
		foreach (var recruitModel in models)
		{
			var existingEntity = _context.Recruits.Find(recruitModel.Id);
			if (existingEntity == null) newRecruits.Add(recruitModel);
			else Update(existingEntity, recruitModel);
		}

		_context.SaveChanges();

		using (var context = new DefaultContext(_connectionString))
		{
			context.Recruits.AddRange(newRecruits);

			context.Database.OpenConnection();
			try
			{
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Recruits ON");
				context.SaveChanges();
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Recruits OFF");
			}
			finally
			{
				context.Database.CloseConnection();
			}
		}
	}

	public void ImportRecruitQuestions(List<RecruitQuestion> models)
	{
		var newRecruitQuestions = new List<RecruitQuestion>();
		foreach (var recruitQuestionModel in models)
		{
			var existingEntity = _context.RecruitQuestions.FirstOrDefault(x => x.RecruitId == recruitQuestionModel.RecruitId && x.QuestionId == recruitQuestionModel.QuestionId);
			if (existingEntity == null) newRecruitQuestions.Add(recruitQuestionModel);
		}
		_context.RecruitQuestions.AddRange(newRecruitQuestions);
		_context.SaveChanges();

	}

	public void ImportNotes(List<Note> models)
	{
		var newNotes = new List<Note>();
		foreach (var noteModel in models)
		{

			var existingEntity = _context.Notes.Find(noteModel.Id);
			if (existingEntity == null) newNotes.Add(noteModel);
			else Update(existingEntity, noteModel);
		}

		_context.SaveChanges();

		using (var context = new DefaultContext(_connectionString))
		{
			context.Notes.AddRange(newNotes);

			context.Database.OpenConnection();
			try
			{
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Notes ON");
				context.SaveChanges();
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Notes OFF");
			}
			finally
			{
				context.Database.CloseConnection();
			}
		}

	}

	public void ImportArticles(List<Article> models)
	{
		var newArticles = new List<Article>();
		foreach (var articleModel in models)
		{
			var existingEntity = _context.Articles.Find(articleModel.Id);
			if (existingEntity == null) newArticles.Add(articleModel);
			else Update(existingEntity, articleModel);
		}

		_context.SaveChanges();

		using (var context = new DefaultContext(_connectionString))
		{
			context.Articles.AddRange(newArticles);

			context.Database.OpenConnection();
			try
			{
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Articles ON");
				context.SaveChanges();
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Articles OFF");
			}
			finally
			{
				context.Database.CloseConnection();
			}
		}

	}

	public void ImportManuals(List<Manual> models)
	{
		var newManuals = new List<Manual>();
		foreach (var manualModel in models)
		{

			var existingEntity = _context.Manuals.Find(manualModel.Id);
			if (existingEntity == null) newManuals.Add(manualModel);
			else Update(existingEntity, manualModel);
		}

		_context.SaveChanges();

		using (var context = new DefaultContext(_connectionString))
		{
			context.Manuals.AddRange(newManuals);

			context.Database.OpenConnection();
			try
			{
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Manuals ON");
				context.SaveChanges();
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Manuals OFF");
			}
			finally
			{
				context.Database.CloseConnection();
			}
		}
	}
	public void ImportFeatures(List<Feature> models)
	{
		var newFeatures = new List<Feature>();
		foreach (var featureModel in models)
		{

			var existingEntity = _context.Features.Find(featureModel.Id);
			if (existingEntity == null) newFeatures.Add(featureModel);
			else Update(existingEntity, featureModel);
		}

		_context.SaveChanges();

		using (var context = new DefaultContext(_connectionString))
		{
			context.Features.AddRange(newFeatures);

			context.Database.OpenConnection();
			try
			{
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Features ON");
				context.SaveChanges();
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT Features OFF");
			}
			finally
			{
				context.Database.CloseConnection();
			}
		}
	}

	public void ImportUploadFiles(List<UploadFile> models)
	{
		var newUploadFiles = new List<UploadFile>();
		foreach (var uploadFileModel in models)
		{
			var existingEntity = _context.UploadFiles.Find(uploadFileModel.Id);
			if (existingEntity == null) newUploadFiles.Add(uploadFileModel);
			else Update(existingEntity, uploadFileModel);
		}

		_context.SaveChanges();

		using (var context = new DefaultContext(_connectionString))
		{
			context.UploadFiles.AddRange(newUploadFiles);

			context.Database.OpenConnection();
			try
			{
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT UploadFiles ON");
				context.SaveChanges();
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT UploadFiles OFF");
			}
			finally
			{
				context.Database.CloseConnection();
			}
		}

	}

	public void ImportReviewRecords(List<ReviewRecord> models)
	{
		var newReviewRecords = new List<ReviewRecord>();
		foreach (var reviewRecordModel in models)
		{
			var existingEntity = _context.ReviewRecords.Find(reviewRecordModel.Id);
			if (existingEntity == null) newReviewRecords.Add(reviewRecordModel);
			else Update(existingEntity, reviewRecordModel);
		}

		_context.SaveChanges();

		using (var context = new DefaultContext(_connectionString))
		{
			context.ReviewRecords.AddRange(newReviewRecords);

			context.Database.OpenConnection();
			try
			{
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT ReviewRecords ON");
				context.SaveChanges();
				context.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT ReviewRecords OFF");
			}
			finally
			{
				context.Database.CloseConnection();
			}
		}

	}

	void Update(EntityBase existingEntity, EntityBase model)
	{
		var entry = _context.Entry(existingEntity);
		entry.CurrentValues.SetValues(model);
		entry.State = EntityState.Modified;
	}

	public void SyncSubjects(List<Subject> models)
	{
		var ids = models.Select(x => x.Id).ToList();

		var deletedEntities = _context.Subjects.Where(x => !ids.Contains(x.Id)).ToList();

		if (deletedEntities.HasItems()) _context.Subjects.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

	public void SyncTerms(List<Term> models)
	{
		var ids = models.Select(x => x.Id).ToList();

		var deletedEntities = _context.Terms.Where(x => !ids.Contains(x.Id)).ToList();

		if (deletedEntities.HasItems()) _context.Terms.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}


	public void SyncQuestions(List<Question> models)
	{
		var ids = models.Select(x => x.Id).ToList();

		var deletedEntities = _context.Questions.Where(x => !ids.Contains(x.Id)).ToList();

		if (deletedEntities.HasItems()) _context.Questions.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

	public void SyncOptions(List<Option> models)
	{
		var ids = models.Select(x => x.Id).ToList();

		var deletedEntities = _context.Options.Where(x => !ids.Contains(x.Id)).ToList();

		if (deletedEntities.HasItems()) _context.Options.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

	public void SyncTermQuestions(List<TermQuestion> models)
	{
		var deletedEntities = new List<TermQuestion>();

		foreach (var existingItem in _context.TermQuestions)
		{
			var newItem = models.FirstOrDefault(x => x.TermId == existingItem.TermId && x.QuestionId == existingItem.QuestionId);
			if (newItem == null) deletedEntities.Add(existingItem);
		}

		if (deletedEntities.HasItems()) _context.TermQuestions.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

	public void SyncResolves(List<Resolve> models)
	{
		var ids = models.Select(x => x.Id).ToList();

		var deletedEntities = _context.Resolves.Where(x => !ids.Contains(x.Id)).ToList();

		if (deletedEntities.HasItems()) _context.Resolves.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

	public void SyncRecruits(List<Recruit> models)
	{
		var ids = models.Select(x => x.Id).ToList();

		var deletedEntities = _context.Recruits.Where(x => !ids.Contains(x.Id)).ToList();

		if (deletedEntities.HasItems()) _context.Recruits.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

	public void SyncRecruitQuestions(List<RecruitQuestion> models)
	{
		var deletedEntities = new List<RecruitQuestion>();

		foreach (var existingItem in _context.RecruitQuestions)
		{
			var newItem = models.FirstOrDefault(x => x.RecruitId == existingItem.RecruitId && x.QuestionId == existingItem.QuestionId);
			if (newItem == null) deletedEntities.Add(existingItem);
		}

		if (deletedEntities.HasItems()) _context.RecruitQuestions.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

	public void SyncNotes(List<Note> models)
	{
		var ids = models.Select(x => x.Id).ToList();

		var deletedEntities = _context.Notes.Where(x => !ids.Contains(x.Id)).ToList();

		if (deletedEntities.HasItems()) _context.Notes.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

	public void SyncArticles(List<Article> models)
	{
		var ids = models.Select(x => x.Id).ToList();

		var deletedEntities = _context.Articles.Where(x => !ids.Contains(x.Id)).ToList();

		if (deletedEntities.HasItems()) _context.Articles.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

	public void SyncManuals(List<Manual> models)
	{
		var ids = models.Select(x => x.Id).ToList();

		var deletedEntities = _context.Manuals.Where(x => !ids.Contains(x.Id)).ToList();

		if (deletedEntities.HasItems()) _context.Manuals.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

	public void SyncFeatures(List<Feature> models)
	{
		var ids = models.Select(x => x.Id).ToList();

		var deletedEntities = _context.Features.Where(x => !ids.Contains(x.Id)).ToList();

		if (deletedEntities.HasItems()) _context.Features.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

	public void SyncUploadFiles(List<UploadFile> models)
	{
		var ids = models.Select(x => x.Id).ToList();

		var deletedEntities = _context.UploadFiles.Where(x => !ids.Contains(x.Id)).ToList();

		if (deletedEntities.HasItems()) _context.UploadFiles.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

	public void SyncReviewRecords(List<ReviewRecord> models)
	{
		var ids = models.Select(x => x.Id).ToList();

		var deletedEntities = _context.ReviewRecords.Where(x => !ids.Contains(x.Id)).ToList();

		if (deletedEntities.HasItems()) _context.ReviewRecords.RemoveRange(deletedEntities);

		_context.SaveChanges();
	}

}
