using ApplicationCore.Views;
using ApplicationCore.Models;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;

namespace ApplicationCore.Helpers;

public static class PlansHelpers
{
	public static async Task<IEnumerable<Plan>> FetchAllAsync(this IDefaultRepository<Plan> plansRepository)
		=> await plansRepository.ListAsync(new PlansSpecification());
	public static async Task<IEnumerable<Plan>?> FetchAsync(this IDefaultRepository<Plan> plansRepository, bool active)
	{
		var plans = await plansRepository.FetchAllAsync();
		if (plans.IsNullOrEmpty()) return null;

		return plans.Where(x => x.Active == active);
	}
	public static PlanViewModel MapViewModel(this Plan plan, IMapper mapper, bool discount = false)
	{ 
	    var model = mapper.Map<PlanViewModel>(plan);
		if (discount) 
		{
			model.Price = Convert.ToInt32(plan.Money * plan.Discount / 100);
		}
		else model.Price = Convert.ToInt32(plan.Money);

		model.StartDateText = plan.StartDate.ToDateString();
		model.EndDateText = plan.EndDate.ToDateString();

		return model;
	}

	public static List<PlanViewModel> MapViewModelList(this IEnumerable<Plan> plans, IMapper mapper)
		=> plans.Select(item => MapViewModel(item, mapper)).ToList();

	
	public static Plan MapEntity(this PlanViewModel model, IMapper mapper, string currentUserId, Plan? entity = null)
	{
		if (entity == null) entity = mapper.Map<PlanViewModel, Plan>(model);
		else entity = mapper.Map<PlanViewModel, Plan>(model, entity);
		
		entity.Money = Convert.ToDecimal(model.Price);
		entity.StartDate = model.StartDateText!.ToStartDate();
		entity.EndDate = model.EndDateText!.ToEndDate();

		if (model.Id == 0) entity.SetCreated(currentUserId);
		entity.SetUpdated(currentUserId);

		return entity;
	}

	public static IEnumerable<Plan> GetOrdered(this IEnumerable<Plan> plans)
		=> plans.HasItems() ? plans.OrderByDescending(item => item.StartDate)
							: new List<Plan>();

}
