using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Paging;
using AutoMapper;
using Newtonsoft.Json;
using ApplicationCore.Views;

namespace ApplicationCore.Helpers;
public static class ReceiversHelpers
{
	public static async Task<IEnumerable<Receiver>> FetchByUserAsync(this IDefaultRepository<Receiver> receiverRepository, User user)
		=> await receiverRepository.ListAsync(new ReceiversSpecification(user));
	public static async Task<Receiver?> GetUserNotificationByIdAsync(this IDefaultRepository<Receiver> receiverRepository, int id)
		=> await receiverRepository.FirstOrDefaultAsync(new ReceiversSpecification(id));
	public static async Task ClearUserNotificationsAsync(this IDefaultRepository<Receiver> receiverRepository, User user, ICollection<int> ids)
	{
		var receivers = await receiverRepository.FetchByUserAsync(user);

		var items = receivers.Where(item => ids.Contains(item.Id)).ToList();
		if (items.HasItems())
		{
			foreach (var item in items) item.ReceivedAt = DateTime.Now;
			await receiverRepository.UpdateRangeAsync(items);
		}
	}
	public static ReceiverViewModel MapViewModel(this Receiver receiver, IMapper mapper)
	{
		var model = mapper.Map<ReceiverViewModel>(receiver);
		if (receiver.Notice != null) model.Notice = receiver.Notice.MapViewModel(mapper);

		return model;
	}

	public static List<ReceiverViewModel> MapViewModelList(this IEnumerable<Receiver> receivers, IMapper mapper)
		=> receivers.Select(item => MapViewModel(item, mapper)).ToList();


	public static IEnumerable<Receiver> GetOrdered(this IEnumerable<Receiver> receivers)
		=> receivers.OrderByDescending(item => item.Notice!.LastUpdated);


	public static PagedList<Receiver, ReceiverViewModel> GetPagedList(this IEnumerable<Receiver> receivers, IMapper mapper, int page = 1, int pageSize = 999)
	{
		var pageList = new PagedList<Receiver, ReceiverViewModel>(receivers, page, pageSize);

		pageList.SetViewList(pageList.List.MapViewModelList(mapper));

		return pageList;
	}
}
