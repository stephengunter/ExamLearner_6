using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;

namespace ApplicationCore.Helpers;

public static class PayWaysHelpers
{
	public static async Task<IEnumerable<PayWay>> FetchAllAsync(this IDefaultRepository<PayWay> paywaysRepository)
		=> await paywaysRepository.ListAsync(new PayWaysSpecification());
	public static async Task<IEnumerable<PayWay>?> FetchAsync(this IDefaultRepository<PayWay> paywaysRepository, bool active = true)
	{
		var payWays = await paywaysRepository.FetchAllAsync();
		if (payWays.IsNullOrEmpty()) return null;

		return payWays.Where(x => x.Active == active);
	}
	public static async Task<PayWay?> FindAsync(this IDefaultRepository<PayWay> paywaysRepository, int id)
		=> await paywaysRepository.FirstOrDefaultAsync(new PayWaysSpecification(id));
	public static async Task<PayWay?> FindByCodeAsync(this IDefaultRepository<PayWay> paywaysRepository, string code)
		=> await paywaysRepository.FirstOrDefaultAsync(new PayWaysSpecification(code));
	public static IEnumerable<PayWay> GetOrdered(this IEnumerable<PayWay> payWays)
		=> payWays.OrderBy(item => item.Order);
}
