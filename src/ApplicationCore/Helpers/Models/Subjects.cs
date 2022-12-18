using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Views;
using AutoMapper;
using Infrastructure.Views;

namespace ApplicationCore.Helpers;

public static class SubjectsHelpers
{
	public static async Task<IEnumerable<Subject>> FetchAllAsync(this IDefaultRepository<Subject> subjectsRepository)
		=> await subjectsRepository.ListAsync(new SubjectsSpecification());
	public static async Task<IEnumerable<Subject>> FetchAsync(this IDefaultRepository<Subject> subjectsRepository, int parentId = -1)
	{
		if (parentId < 0) return await subjectsRepository.FetchAllAsync();
		else return await subjectsRepository.ListAsync(new SubjectsSpecification(parentId));
	}
	public static async Task<Subject?> FindSubjectLoadSubItemsAsync(this IDefaultRepository<Subject> subjectsRepository, int id)
	{
		var subject = await subjectsRepository.FirstOrDefaultAsync(new SubjectsSpecification(id));
		if (subject != null) subject.LoadSubItems(subjectsRepository.DbSet.AllSubItems());

		return subject;
	}
	public static void LoadSubItems(this IDefaultRepository<Subject> subjectsRepository, IEnumerable<Subject> subjects)
	{
		var subItems = subjectsRepository.DbSet.AllSubItems();
		foreach (var entity in subjects)
		{
			entity.LoadSubItems(subItems);
		}
	}
	public static async Task<IEnumerable<Subject>> FetchExamSubjectsAsync(this IDefaultRepository<Subject> subjectsRepository)
	{
		int parentId = 0;
		return await subjectsRepository.ListAsync(new SubjectsSpecification(parentId));
	}

	public static SubjectViewModel MapViewModel(this Subject subject, IMapper mapper)
		=> mapper.Map<SubjectViewModel>(subject);

	public static List<SubjectViewModel> MapViewModelList(this IEnumerable<Subject> subjects, IMapper mapper)
		=> subjects.Select(item => MapViewModel(item, mapper)).ToList();

	public static Subject MapEntity(this SubjectViewModel model, IMapper mapper, string currentUserId, Subject? entity = null)
	{
		if (entity == null) entity = mapper.Map<SubjectViewModel, Subject>(model);
		else entity = mapper.Map<SubjectViewModel, Subject>(model, entity);

		if (model.Id == 0) entity.SetCreated(currentUserId);
		else entity.SetUpdated(currentUserId);

		return entity;
	}
	public static IEnumerable<Subject> GetOrdered(this IEnumerable<Subject> subjects)
		=> subjects.OrderBy(item => item.Order);

	public static BaseOption<int> ToOption(this Subject subject)
		=> new BaseOption<int>(subject.Id, subject.Title);
}
