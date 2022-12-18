using ApplicationCore.Paging;
using ApplicationCore.Views;

namespace ApplicationCore.Helpers;

public static class ExceptionsHelpers
{
	public static PagedList<ExceptionViewModel> GetPagedList(this IEnumerable<ExceptionViewModel> records, int page = 1, int pageSize = 999)
		=> new PagedList<ExceptionViewModel>(records, page, pageSize);

	public static IEnumerable<ExceptionViewModel> GetOrdered(this IEnumerable<ExceptionViewModel> list)
		=> list.OrderByDescending(item => item.DateTime);
}
