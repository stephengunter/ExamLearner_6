using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;

namespace ApplicationCore.Helpers;

public static class BillsHelpers
{
	public static async Task<IEnumerable<Bill>> FetchAllAsync(this IDefaultRepository<Bill> billsRepository)
		=> await billsRepository.ListAsync(new BillsSpecification());
	public static async Task<IEnumerable<Bill>> FetchByUserAsync(this IDefaultRepository<Bill> billsRepository, User user)
		=> await billsRepository.ListAsync(new BillsSpecification(user));
	public static async Task<IEnumerable<Bill>> FetchByUserAsync(this IDefaultRepository<Bill> billsRepository, User user, Plan plan)
		=> await billsRepository.ListAsync(new BillsSpecification(user, plan));
	public static async Task<IEnumerable<Bill>> FetchByPlanAsync(this IDefaultRepository<Bill> billsRepository, Plan plan)
		=> await billsRepository.ListAsync(new BillsByPlanSpecification(plan));
	public static async Task<Bill?> FindBillIncludeItemsAsync(this IDefaultRepository<Bill> billsRepository, int id)
		=> await billsRepository.FirstOrDefaultAsync(new BillsSpecification(id));
	public static async Task RemoveAsync(this IDefaultRepository<Bill> billsRepository, Bill bill)
	{
		bill.Removed = true;

		foreach (var pay in bill.Pays!)
		{
			pay.Removed = true;
		}

		await billsRepository.UpdateAsync(bill);
	}
	public static BillViewModel MapViewModel(this Bill bill, IMapper mapper, IEnumerable<PayWay>? payWays = null)
	{
		var model = mapper.Map<BillViewModel>(bill);

		if (bill.Plan != null) model.Plan = bill.Plan.MapViewModel(mapper, bill.HasDiscount);
		if (payWays!.HasItems()) model.Pays = bill.Pays!.MapViewModelList(mapper, payWays);

		return model;
	}

	public static List<BillViewModel> MapViewModelList(this IEnumerable<Bill> bills, IMapper mapper, IEnumerable<PayWay>? payWays = null)
		=> bills.Select(item => MapViewModel(item, mapper, payWays)).ToList();

	public static PagedList<Bill, BillViewModel> GetPagedList(this IEnumerable<Bill> bills, IMapper mapper, int page = 1, int pageSize = 999)
	{
		var pageList = new PagedList<Bill, BillViewModel>(bills, page, pageSize);

		pageList.SetViewList(pageList.List.MapViewModelList(mapper));

		return pageList;
	}


	public static Bill MapEntity(this BillViewModel model, IMapper mapper, string currentUserId)
	{
		var entity = mapper.Map<BillViewModel, Bill>(model);

		if (model.Id == 0) entity.SetCreated(currentUserId);
		entity.SetUpdated(currentUserId);

		return entity;
	}

	public static IEnumerable<Bill> GetOrdered(this IEnumerable<Bill> bills)
	{
		return bills.OrderByDescending(item => item.CreatedAt);

	}

}
