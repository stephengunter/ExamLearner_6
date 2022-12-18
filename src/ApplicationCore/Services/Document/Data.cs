using ApplicationCore.Models.Data;
using ApplicationCore.Views;
using ApplicationCore.Helpers;
using Newtonsoft.Json;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications.Data;
using ApplicationCore.Models;

namespace ApplicationCore.Services;

public interface IDataService
{
	Task<NoteParamsViewModel?> FindNoteParamsAsync(string userId);
	Task SaveNoteParamsAsync(string userId, NoteParamsViewModel model);

	Task<ExamSettingsViewModel?> FindExamSettingsAsync(int subjectId);
	Task SaveExamSettingsAsync(int subjectId, ExamSettingsViewModel model);

	Task<IEnumerable<SubjectQuestionsViewModel>?> FindSubjectQuestionsAsync(int subjectId);
	Task SaveSubjectQuestionsAsync(int subjectId, IEnumerable<SubjectQuestionsViewModel> models);

	Task<IEnumerable<RecruitViewModel>?> FetchYearRecruitsAsync();
	Task SaveYearRecruitsAsync(IEnumerable<RecruitViewModel> models);

	Task<IEnumerable<NoteCategoryViewModel>?> FetchNoteCategoriesAsync();
	Task SaveNoteCategoriesAsync(IEnumerable<NoteCategoryViewModel> models);

	Task<IEnumerable<TermViewModel>?> FetchTermNotesBySubjectAsync(Subject subject);
	Task<TermViewModel?> FindTermNotesByTermAsync(Term term);
	Task<TermNotes?> FindTermNotesViewByTermAsync(Term term);
	Task<IEnumerable<TermNotes>?> FetchTermNotesViewBySubjectAsync(Subject subject);
	Task CleanTermNotesAsync();
	Task SaveTermNotesAsync(TermViewModel model, List<NoteViewModel> noteViewList, List<int> RQIds, List<int> qIds);

}

public class DataService : IDataService
{
	private readonly IDefaultRepository<NoteParams> _noteParamsRepository;
	private readonly IDefaultRepository<ExamSettings> _examSettingsRepository;
	private readonly IDefaultRepository<SubjectQuestions> _subjectQuestionsRepository;
	private readonly IDefaultRepository<YearRecruit> _yearRecruitsRepository;
	private readonly IDefaultRepository<NoteCategories> _noteCategoriesRepository;
	private readonly IDefaultRepository<TermNotes> _termNotesRepository;

	public DataService(IDefaultRepository<NoteParams> noteParamsRepository,
		 IDefaultRepository<ExamSettings> examSettingsRepository, IDefaultRepository<SubjectQuestions> subjectQuestionsRepository,
		 IDefaultRepository<YearRecruit> yearRecruitsRepository, IDefaultRepository<NoteCategories> noteCategoriesRepository,
		 IDefaultRepository<TermNotes> termNotesRepository)
	{
		_noteParamsRepository = noteParamsRepository;
		_examSettingsRepository = examSettingsRepository;
		_subjectQuestionsRepository = subjectQuestionsRepository;
		_yearRecruitsRepository = yearRecruitsRepository;
		_noteCategoriesRepository = noteCategoriesRepository;
		_termNotesRepository = termNotesRepository;
	}

	public async Task SaveNoteParamsAsync(string userId, NoteParamsViewModel model)
	{
		var existingDoc = await _noteParamsRepository.FirstOrDefaultAsync(new NoteParamsSpecification(userId));
		string content = JsonConvert.SerializeObject(model);

		if (existingDoc == null) await _noteParamsRepository.AddAsync(new NoteParams { UserId = userId, Content = content });
		else
		{

			existingDoc.Content = content;
			existingDoc.LastUpdated = DateTime.Now;
			await _noteParamsRepository.UpdateAsync(existingDoc);
		}
	}

	public async Task<NoteParamsViewModel?> FindNoteParamsAsync(string userId)
	{
		var doc = await _noteParamsRepository.FirstOrDefaultAsync(new NoteParamsSpecification(userId));
		if (doc == null) return null;

		return JsonConvert.DeserializeObject<NoteParamsViewModel>(doc.Content);
	}
	public async Task SaveExamSettingsAsync(int subjectId, ExamSettingsViewModel model)
	{
		var existingDoc = await _examSettingsRepository.FirstOrDefaultAsync(new ExamSettingsSpecification(subjectId));
		string content = JsonConvert.SerializeObject(model);

		if (existingDoc == null) await _examSettingsRepository.AddAsync(new ExamSettings { SubjectId = subjectId, Content = content });
		else
		{

			existingDoc.Content = content;
			existingDoc.LastUpdated = DateTime.Now;
			await _examSettingsRepository.UpdateAsync(existingDoc);
		}
	}

	public async Task<ExamSettingsViewModel?> FindExamSettingsAsync(int subjectId)
	{
		var doc = await _examSettingsRepository.FirstOrDefaultAsync(new ExamSettingsSpecification(subjectId));
		if (doc == null) return null;

		return JsonConvert.DeserializeObject<ExamSettingsViewModel>(doc.Content);

	}

	public async Task<IEnumerable<SubjectQuestionsViewModel>?> FindSubjectQuestionsAsync(int subjectId)
	{
		var doc = await _subjectQuestionsRepository.FirstOrDefaultAsync(new SubjectQuestionsSpecification(subjectId));
		if (doc == null) return null;

		return JsonConvert.DeserializeObject<IEnumerable<SubjectQuestionsViewModel>>(doc.Content);
	}

	public async Task SaveSubjectQuestionsAsync(int subjectId, IEnumerable<SubjectQuestionsViewModel> models)
	{
		var existingDoc = await _subjectQuestionsRepository.FirstOrDefaultAsync(new SubjectQuestionsSpecification(subjectId));
		string content = JsonConvert.SerializeObject(models);

		if (existingDoc == null) await _subjectQuestionsRepository.AddAsync(new SubjectQuestions { SubjectId = subjectId, Content = content });
		else
		{

			existingDoc.Content = content;
			existingDoc.LastUpdated = DateTime.Now;
			await _subjectQuestionsRepository.UpdateAsync(existingDoc);
		}
	}

	public async Task<IEnumerable<RecruitViewModel>?> FetchYearRecruitsAsync()
	{
		var docs = await _yearRecruitsRepository.ListAsync();
		if (docs.IsNullOrEmpty()) return null;

		return docs.Select(doc => JsonConvert.DeserializeObject<RecruitViewModel>(doc.Content))!;
	}

	public async Task SaveYearRecruitsAsync(IEnumerable<RecruitViewModel> models)
	{
		var exitingItems = await _yearRecruitsRepository.ListAsync();
		if (exitingItems.HasItems()) await _yearRecruitsRepository.DeleteRangeAsync(exitingItems);


		var docs = models.Select(model => new YearRecruit { Content = JsonConvert.SerializeObject(model) });
		await _yearRecruitsRepository.AddRangeAsync(docs.ToList());
	}

	public async Task<IEnumerable<NoteCategoryViewModel>?> FetchNoteCategoriesAsync()
	{
		var docs = await _noteCategoriesRepository.ListAsync();
		if (docs.IsNullOrEmpty()) return null;

		return docs.Select(doc => JsonConvert.DeserializeObject<NoteCategoryViewModel>(doc.Content))!;
	}

	public async Task SaveNoteCategoriesAsync(IEnumerable<NoteCategoryViewModel> models)
	{
		var exitingItems = await _noteCategoriesRepository.ListAsync();
		if (exitingItems.HasItems()) await _noteCategoriesRepository.DeleteRangeAsync(exitingItems);

		var docs = models.Select(model => new NoteCategories { Content = JsonConvert.SerializeObject(model) });
		await _noteCategoriesRepository.AddRangeAsync(docs.ToList());
	}

	public async Task<IEnumerable<TermViewModel>?> FetchTermNotesBySubjectAsync(Subject subject)
	{
		var docs = await _termNotesRepository.ListAsync(new TermNotesSpecification(subject));
		if (docs.IsNullOrEmpty()) return null;

		return docs.Select(doc => JsonConvert.DeserializeObject<TermViewModel>(doc.Content))!;
	}

	public async Task<TermViewModel?> FindTermNotesByTermAsync(Term term)
	{
		var doc = await _termNotesRepository.FirstOrDefaultAsync(new TermNotesSpecification(term));
		if (doc == null) return null;

		return JsonConvert.DeserializeObject<TermViewModel>(doc.Content);
	}

	public async Task<TermNotes?> FindTermNotesViewByTermAsync(Term term) 
		=> await _termNotesRepository.FirstOrDefaultAsync(new TermNotesSpecification(term));
	public async Task<IEnumerable<TermNotes>?> FetchTermNotesViewBySubjectAsync(Subject subject)
		 => await _termNotesRepository.ListAsync(new TermNotesSpecification(subject));

	public async Task CleanTermNotesAsync()
	{
		var exitingItems = await _termNotesRepository.ListAsync();
		if (exitingItems.HasItems()) await _termNotesRepository.DeleteRangeAsync(exitingItems);
	}

	public async Task SaveTermNotesAsync(TermViewModel model, List<NoteViewModel> noteViewList, List<int> RQIds, List<int> qIds)
	{
		int termId = model.Id;
		int subjectId = model.SubjectId;

		model.Subject = null;
		if (model.SubItems!.HasItems()) foreach (var item in model.SubItems!) item.Subject = null;

		model.LoadNotes(noteViewList);

		var termNote = new TermNotes
		{
			SubjectId = subjectId,
			TermId = termId,
			Content = JsonConvert.SerializeObject(model),
			RQIds = RQIds.JoinToStringIntegers(),
			QIds = qIds.JoinToStringIntegers()
		};


		await _termNotesRepository.AddAsync(termNote);
	}

}
