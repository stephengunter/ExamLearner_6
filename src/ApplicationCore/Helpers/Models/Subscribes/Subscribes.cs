using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;

namespace ApplicationCore.Helpers;

public static class SubscribesHelpers
{
	public static async Task<IEnumerable<Subscribe>> FetchByUserAsync(this IDefaultRepository<Subscribe> subscribesRepository, User user)
		=> await subscribesRepository.ListAsync(new SubscribesSpecification(user));
	public static async Task<IEnumerable<Subscribe>> FetchByPlanAsync(this IDefaultRepository<Subscribe> subscribesRepository, Plan plan)
		=> await subscribesRepository.ListAsync(new SubscribesSpecification(plan));
	public static async Task<Subscribe?> FindByBillAsync(this IDefaultRepository<Subscribe> subscribesRepository, Bill bill)
		=> await subscribesRepository.FirstOrDefaultAsync(new SubscribesSpecification(bill));
	public static SubscribeViewModel MapViewModel(this Subscribe subscribe, IMapper mapper)
	{ 
	    var model = mapper.Map<SubscribeViewModel>(subscribe);
		
		model.StartDateText = subscribe.StartDate.ToDateString();
		model.EndDateText = subscribe.EndDate.ToDateString();
		return model;
	}

	public static List<SubscribeViewModel> MapViewModelList(this IEnumerable<Subscribe> subscribes, IMapper mapper)
		=> subscribes.Select(item => MapViewModel(item, mapper)).ToList();


	public static PagedList<Subscribe, SubscribeViewModel> GetPagedList(this IEnumerable<Subscribe> subscribes, IMapper mapper, int page = 1, int pageSize = 999)
	{
		var pageList = new PagedList<Subscribe, SubscribeViewModel>(subscribes, page, pageSize);

		pageList.SetViewList(pageList.List.MapViewModelList(mapper));

		return pageList;
	}


	public static Subscribe MapEntity(this SubscribeViewModel model, IMapper mapper, string currentUserId)
	{
		var entity = mapper.Map<SubscribeViewModel, Subscribe>(model);
		
		entity.StartDate = model.StartDateText!.ToStartDate();
		entity.EndDate = model.EndDateText!.ToStartDate();

		if (model.Id == 0) entity.SetCreated(currentUserId);
		entity.SetUpdated(currentUserId);

		return entity;
	}

	public static IEnumerable<Subscribe> GetOrdered(this IEnumerable<Subscribe> subscribes)
		=> subscribes.HasItems() ? subscribes.OrderByDescending(item => item.StartDate)
								 : new List<Subscribe>();

}
