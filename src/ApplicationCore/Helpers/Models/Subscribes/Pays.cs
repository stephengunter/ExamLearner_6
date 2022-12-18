using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;

namespace ApplicationCore.Helpers;

public static class PaysHelpers
{
	public static async Task<IEnumerable<Pay>> FetchAllAsync(this IDefaultRepository<Pay> paysRepository)
		=> await paysRepository.ListAsync(new PaysSpecification());
	public static async Task<IEnumerable<Pay>> FetchByBillsAsync(this IDefaultRepository<Pay> paysRepository, ICollection<int> billIds)
		=> await paysRepository.ListAsync(new PaysByBillsSpecification(billIds));
	public static async Task<Pay?> FindByCodeAsync(this IDefaultRepository<Pay> paysRepository, string code)
		=> await paysRepository.FirstOrDefaultAsync(new PaysSpecification(code));
	public static PayViewModel MapViewModel(this Pay pay, IMapper mapper, IEnumerable<PayWay>? payWays = null)
	{
		var model = mapper.Map<PayViewModel>(pay);

		if (payWays!.HasItems())
		{
			var payway = payWays!.FirstOrDefault(x => x.Code == pay.PayWay);
			if (payway != null) model.PayWayTitle = payway.Title;
		}

		return model;
	}

	public static PayWayViewModel MapViewModel(this PayWay payWay, IMapper mapper)
		=> mapper.Map<PayWayViewModel>(payWay);


	public static List<PayViewModel> MapViewModelList(this IEnumerable<Pay> pays, IMapper mapper, IEnumerable<PayWay>? payWays = null)
		=> pays.Select(item => MapViewModel(item, mapper, payWays)).ToList();

	public static List<PayWayViewModel> MapViewModelList(this IEnumerable<PayWay> payWays, IMapper mapper)
		=> payWays.Select(item => MapViewModel(item, mapper)).ToList();


	public static PagedList<Pay, PayViewModel> GetPagedList(this IEnumerable<Pay> pays, IMapper mapper, IEnumerable<PayWay>? payWays = null, int page = 1, int pageSize = 999)
	{
		var pageList = new PagedList<Pay, PayViewModel>(pays, page, pageSize);

		pageList.SetViewList(pageList.List.MapViewModelList(mapper, payWays));

		return pageList;
	}

	public static PayWay MapEntity(this PayWayViewModel model, IMapper mapper, string currentUserId)
	{
		var entity = mapper.Map<PayWayViewModel, PayWay>(model);

		if (model.Id == 0) entity.SetCreated(currentUserId);
		entity.SetUpdated(currentUserId);

		return entity;
	}

	public static IEnumerable<Pay> GetOrdered(this IEnumerable<Pay> pays)
		=> pays.OrderByDescending(item => item.CreatedAt);

	

}
