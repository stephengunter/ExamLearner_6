using Infrastructure.Entities;
using Infrastructure.Interfaces;

namespace ApplicationCore.Helpers;
public static class BaseRecordsHelpers
{
	public static async Task UpdateOrderAsync<TEntity>(this IRepository<TEntity> repository, int target, int replace, bool up)
		 where TEntity : BaseRecord
	{
		var targetEntity = await repository.GetByIdAsync(target);
		int targetOrder = targetEntity!.Order;

		var replaceEntity = await repository.GetByIdAsync(replace);
		int replaceOrder = replaceEntity!.Order;

		targetEntity.Order = replaceOrder;
		replaceEntity.Order = targetOrder;

		if (targetEntity.Order == replaceEntity.Order)
		{
			if (up) replaceEntity.Order += 1;
			else targetEntity.Order += 1;
		}

		await repository.UpdateRangeAsync(new List<TEntity> { targetEntity, replaceEntity });
	}
}
