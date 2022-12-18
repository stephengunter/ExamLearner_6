using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Exceptions;
using ApplicationCore.Views;
using AutoMapper;
using ApplicationCore.Paging;

namespace ApplicationCore.Helpers;
public static class QuestionsHelpers
{
	public static async Task<IEnumerable<Question>> FetchAsync(this IDefaultRepository<Question> repository, Subject subject, ICollection<int>? termIds = null, ICollection<int>? recruitIds = null)
	{
		var subjectIds = subject.GetSubIds();
		subjectIds.Add(subject.Id);

		var spec = new QuestionsBySubjectsSpecification(subjectIds);
		var list = await repository.ListAsync(spec);

		if (termIds!.HasItems())
		{
			var questionIds = repository.FetchQuestionIdsByTerms(termIds!);
			list = list.Where(item => questionIds.Contains(item.Id)).ToList();
		}

		if (recruitIds!.HasItems())
		{
			var questionIds = repository.FetchQuestionIdsByRecruits(recruitIds!);
			list = list.Where(item => questionIds.Contains(item.Id)).ToList();
		}

		return list;
	}
	public static async Task<IEnumerable<Question>> FetchByIdsAsync(this IDefaultRepository<Question> repository, ICollection<int> ids)
		=> await repository.ListAsync(new QuestionsSpecification(ids));

	public static IEnumerable<int> FetchQuestionIdsByTerms(this IDefaultRepository<Question> repository, ICollection<int> termIds)
	{
		var termQuestions = repository.DbContext.TermQuestions.Where(x => termIds.Contains(x.TermId));
		if (termQuestions.IsNullOrEmpty()) return new List<int>();

		return termQuestions.Select(x => x.QuestionId).ToList();
	}
	public static ICollection<int> FetchQuestionIdsByRecruit(this IDefaultRepository<Question> repository, Recruit recruit)
	{
		var recruitQuestions = repository.DbContext.RecruitQuestions.Where(x => x.RecruitId == recruit.Id);
		if (recruitQuestions.IsNullOrEmpty()) return new List<int>();

		return recruitQuestions.Select(x => x.QuestionId).ToList();
	}
	public static ICollection<int> FetchQuestionIdsByRecruits(this IDefaultRepository<Question> repository, ICollection<int> recruitIds)
	{
		var recruitQuestions = repository.DbContext.RecruitQuestions.Where(x => recruitIds.Contains(x.RecruitId));
		if (recruitQuestions.IsNullOrEmpty()) return new List<int>();

		return recruitQuestions.Select(x => x.QuestionId).ToList();
	}
	public static IEnumerable<int> FetchAllRecruitQuestionIds(this IDefaultRepository<Question> repository)
		=> repository.DbContext.RecruitQuestions.Select(item => item.QuestionId);
	public static async Task<Question?> FindQuestionIncludeItemsAsync(this IDefaultRepository<Question> repository, int id)
		=> await repository.FirstOrDefaultAsync(new QuestionsSpecification(id));

	#region MapViewModel
	public static QuestionViewModel MapViewModel(this Question question, IMapper mapper)
		=> mapper.Map<QuestionViewModel>(question);

	static void LoadQuestionData(Question question, ICollection<Recruit>? allRecruits,
		ICollection<UploadFile>? attachmentsList = null, ICollection<Term>? allTerms = null)
	{

		if (question.Resolves!.HasItems()) question.Resolves = question.Resolves!.Where(item => !item.Removed).ToList();

		if (allRecruits!.HasItems())
		{
			if (question.Recruits!.HasItems())
			{
				foreach (var item in question.Recruits!)
				{
					item.LoadParents(allRecruits!);
				}
			}
		}

		if (attachmentsList!.HasItems())
		{
			question.LoadAttachments(attachmentsList!);

			foreach (var option in question.Options)
			{
				option.LoadAttachments(attachmentsList!);
			}

			foreach (var resolve in question.Resolves!)
			{
				resolve.LoadAttachments(attachmentsList!);
			}
		}

		if (allTerms!.HasItems()) question.LoadTerms(allTerms!);

	}

	public static QuestionViewModel MapViewModel(this Question question, IMapper mapper, ICollection<Recruit>? allRecruits = null,
		ICollection<UploadFile>? attachmentsList = null, ICollection<Term>? allTerms = null)

	{
		LoadQuestionData(question, allRecruits, attachmentsList, allTerms);

		var model = mapper.Map<QuestionViewModel>(question);

		if (question.Recruits!.HasItems()) model.Recruits = question.Recruits!.MapViewModelList(mapper);

		if (question.Resolves!.HasItems()) model.Resolves = question.Resolves!.MapViewModelList(mapper, attachmentsList!);

		return model;
	}


	public static List<QuestionViewModel> MapViewModelList(this IEnumerable<Question> questions, IMapper mapper)
		=> questions.Select(item => MapViewModel(item, mapper)).ToList();

	public static List<QuestionViewModel> MapViewModelList(this IEnumerable<Question> questions, IMapper mapper, ICollection<Recruit>? rootRecruits = null,
		ICollection<UploadFile>? attachments = null, ICollection<Term>? allTerms = null)
		=> questions.Select(item => MapViewModel(item, mapper, rootRecruits, attachments, allTerms)).ToList();


	#endregion



	public static PagedList<Question, QuestionViewModel> GetPagedList(this IEnumerable<Question> questions, IMapper mapper, int page = 1, int pageSize = 999)
	{
		var pageList = new PagedList<Question, QuestionViewModel>(questions, page, pageSize);

		pageList.SetViewList(pageList.List.MapViewModelList(mapper));

		return pageList;
	}


	public static PagedList<Question, QuestionViewModel> GetPagedList(this IEnumerable<Question> questions, IMapper mapper, ICollection<Recruit>? allRecruits,
		 ICollection<UploadFile>? attachments = null, ICollection<Term>? allTerms = null, int page = 1, int pageSize = 999)
	{
		var pageList = new PagedList<Question, QuestionViewModel>(questions, page, pageSize);

		pageList.SetViewList(pageList.List.MapViewModelList(mapper, allRecruits, attachments, allTerms));

		return pageList;
	}



	#region MapEntity
	public static Question MapEntity(this QuestionViewModel model, IMapper mapper, string currentUserId, Question? entity = null)
	{
		if (entity == null) entity = mapper.Map<QuestionViewModel, Question>(model);
		else entity = mapper.Map<QuestionViewModel, Question>(model, entity);

		entity.RecruitQuestions = model.Recruits.Select(item => new RecruitQuestion { RecruitId = item.Id }).ToList();

		if (model.Id == 0)
		{
			entity.SetCreated(currentUserId);
		}
		else
		{
			foreach (var option in entity.Options)
			{
				option.QuestionId = entity.Id;
			}
			entity.SetUpdated(currentUserId);
		}

		return entity;
	}

	#endregion


	public static IEnumerable<Question> FilterByKeyword(this IEnumerable<Question> questions, ICollection<string> keywords)
		=> questions.Where(item => keywords.Any(item.Title.CaseInsensitiveContains)).ToList();
	public static ExamQuestion ConversionToExamQuestion(this Question question, int optionCount)
	{
		if (optionCount > question.Options.Count)
		{
			throw new OptionToLessException($"OptionToLess. QuestionId: {question.Id} , OptionCount: {question.Options.Count} , Need: {optionCount} ");
		}
		var options = question.Options.ToList().Shuffle(optionCount);
		var examQuestion = new ExamQuestion
		{
			QuestionId = question.Id,
			Options = options,
			OptionIds = String.Join(",", options.Select(x => x.Id))
		};

		return examQuestion;
	}
}
