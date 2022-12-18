using ApplicationCore.Views;
using ApplicationCore.Models;
using Infrastructure.Views;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;

namespace ApplicationCore.Helpers;

public static class RecruitsHelpers
{
	public static async Task<IEnumerable<Recruit>> FetchAllAsync(this IDefaultRepository<Recruit> recruitsRepository)
		=> await recruitsRepository.ListAsync(new RecruitsSpecification());
	public static async Task<IEnumerable<Recruit>> FetchAsync(this IDefaultRepository<Recruit> recruitsRepository, int parentId = -1)
	{
		if (parentId < 0) return await recruitsRepository.FetchAllAsync();
		else return await recruitsRepository.ListAsync(new RecruitsSpecification(parentId));
	}
	public static async Task<Recruit?> FindRecruitLoadSubItemsAsync(this IDefaultRepository<Recruit> recruitsRepository, int id)
	{
		var recruit = await recruitsRepository.GetByIdAsync(id);
		if (recruit != null) recruit.LoadSubItems(recruitsRepository.DbSet.AllSubItems());

		return recruit;
	}
	public static async Task<Subject?> FindSubjectAsync(this IDefaultRepository<Recruit> recruitsRepository, Recruit recruit, IEnumerable<Subject> subjects)
	{
		if (recruit.RecruitEntityType == RecruitEntityType.Part)
		{
			var parent = await recruitsRepository.GetByIdAsync(recruit.ParentId);
			return subjects.FirstOrDefault(x => x.Id == parent!.SubjectId);
		}
		return subjects.FirstOrDefault(x => x.Id == recruit.SubjectId);
	}
	public static void LoadSubItems(this IDefaultRepository<Recruit> recruitsRepository, IEnumerable<Recruit> recruits)
	{
		var subItems = recruitsRepository.DbSet.AllSubItems();
		foreach (var entity in recruits)
		{
			entity.LoadSubItems(subItems);
		}
	}
	public static void LoadSubItems(this IDefaultRepository<Recruit> recruitsRepository, Recruit recruit)
	{
		var subItems = recruitsRepository.DbSet.AllSubItems();
		recruit.LoadSubItems(subItems);
	}
	public static async Task UpdateAsync(this IDefaultRepository<Recruit> recruitsRepository, Recruit recruit, ICollection<Recruit> subItems)
	{
		int recruitId = recruit.Id;

		await recruitsRepository.UpdateAsync(recruit);
		if (subItems.HasItems())
		{
			foreach (var item in subItems)
			{
				item.ParentId = recruitId;
			}
		}

		var existingSubItems = recruitsRepository.DbSet.Where(x => x.ParentId == recruitId).ToList();

		recruitsRepository.SyncSubItems(existingSubItems, subItems);
	}

	public static RecruitViewModel MapViewModel(this Recruit recruit, IMapper mapper)
	{
		var model = mapper.Map<RecruitViewModel>(recruit);
		model.EntityType = recruit.RecruitEntityType.ToString();
		model.DateText = recruit.Date.ToDateString();
		model.OptionType = recruit.OptionType.ToString();

		if (recruit.SubItems!.HasItems())
		{
			var subjectIds = new List<int>();
			foreach (var item in recruit.SubItems!)
			{
				subjectIds.AddRange(item.SubjectIds);
			}
			model.SubjectIds = subjectIds;


		}

		var parents = new List<RecruitViewModel>();
		var entity = model;
		while (entity.Parent != null)
		{
			parents.Insert(0, entity.Parent);
			entity = entity.Parent;
		}

		model.Parents = parents;



		return model;
	}

	public static List<RecruitViewModel> MapViewModelList(this IEnumerable<Recruit> recruits, IMapper mapper) => recruits.Select(item => MapViewModel(item, mapper)).ToList();

	public static Recruit MapEntity(this RecruitViewModel model, IMapper mapper, string currentUserId, Recruit? entity = null)
	{
		if (entity == null) entity = mapper.Map<RecruitViewModel, Recruit>(model);
		else entity = mapper.Map<RecruitViewModel, Recruit>(model, entity);

		entity.Date = model.DateText!.ToDatetimeOrNull();

		if (model.Id == 0)
		{
			entity.SetCreated(currentUserId);
			foreach (var item in entity.SubItems!)
			{
				item.SetCreated(currentUserId);
			}
		}
		else
		{
			entity.SetUpdated(currentUserId);
			foreach (var item in entity.SubItems!)
			{
				item.SetUpdated(currentUserId);
			}
		}

		return entity;
	}

	public static IEnumerable<Recruit> GetOrdered(this IEnumerable<Recruit> recruits)
		=> recruits.OrderByDescending(item => item.Year).ThenBy(item => item.Order);


	public static BaseOption<int> ToOption(this Recruit recruit)
		=> new BaseOption<int>(recruit.Id, recruit.Title);

	public static BaseOption<int> ToOption(this RecruitViewModel model)
		=> new BaseOption<int>(model.Id, model.Title);


}
