using ApplicationCore.Views;
using ApplicationCore.Models;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;

namespace ApplicationCore.Helpers;

public static class ManualsHelpers
{
	public static async Task<IEnumerable<Manual>> FetchAllAsync(this IDefaultRepository<Manual> manualsRepository)
		=> await manualsRepository.ListAsync(new ManualsSpecification());
	public static async Task<IEnumerable<Manual>> FetchAsync(this IDefaultRepository<Manual> manualsRepository, bool active = true)
	{
		int parentId = 0;
		bool features = true;
		var manuals = await manualsRepository.FetchAsync(parentId, features);
		manuals = manuals.Where(x => x.Active == active).ToList();

		var allItems = await manualsRepository.FetchAllAsync();
		foreach (var item in manuals)
		{
			item.LoadSubItems(allItems);
		}

		return manuals;
	}
	public static async Task<IEnumerable<Manual>> FetchAsync(this IDefaultRepository<Manual> manualsRepository, int parentId, bool features = false)
		=> await manualsRepository.ListAsync(new ManualsByParentSpecification(parentId, features));
	public static ManualViewModel MapViewModel(this Manual manual, IMapper mapper)
	{
		var model = mapper.Map<ManualViewModel>(manual);
		if (model.SubItems!.HasItems()) model.SubItems = model.SubItems!.OrderBy(item => item.Order).ToList();
		if (model.Features.HasItems()) model.Features = model.Features.OrderBy(item => item.Order).ToList();
		return model;
	}

	public static List<ManualViewModel> MapViewModelList(this IEnumerable<Manual> manuals, IMapper mapper)
		=> manuals.Select(item => MapViewModel(item, mapper)).ToList();
	
	public static Manual MapEntity(this ManualViewModel model, IMapper mapper, string currentUserId, Manual? entity = null)
	{
		if (entity == null) entity = mapper.Map<ManualViewModel, Manual>(model);
		else entity = mapper.Map<ManualViewModel, Manual>(model, entity);

		entity.Content = entity.Content!.ReplaceNewLine("");

		if (model.Id == 0) entity.SetCreated(currentUserId);
		entity.SetUpdated(currentUserId);

		return entity;
	}

	public static IEnumerable<Manual> GetOrdered(this IEnumerable<Manual> manuals)
		=> manuals.OrderBy(item => item.Order);


	public static FeatureViewModel MapViewModel(this Feature feature, IMapper mapper)
	{
		var model = mapper.Map<FeatureViewModel>(feature);
		return model;
	}

	public static List<FeatureViewModel> MapViewModelList(this IEnumerable<Feature> features, IMapper mapper)
		=> features.Select(item => MapViewModel(item, mapper)).ToList();
	
	public static Feature MapEntity(this FeatureViewModel model, IMapper mapper, string currentUserId, Feature? entity = null)
	{
		if (entity == null) entity = mapper.Map<FeatureViewModel, Feature>(model);
		else entity = mapper.Map<FeatureViewModel, Feature>(model, entity);

		if (model.Id == 0) entity.SetCreated(currentUserId);
		else entity.SetUpdated(currentUserId);

		return entity;
	}

	public static IEnumerable<Feature> GetOrdered(this IEnumerable<Feature> features)
		=> features.OrderBy(item => item.Order);

}
