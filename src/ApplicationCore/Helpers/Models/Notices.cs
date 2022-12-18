using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Specifications;
using ApplicationCore.Paging;
using AutoMapper;
using Newtonsoft.Json;
using ApplicationCore.Views;

namespace ApplicationCore.Helpers;

public static class NoticesHelpers
{
	public static async Task<Notice> CreateUserNotificationAsync(this IDefaultRepository<Notice> noticesRepository, Notice notice, IEnumerable<string> userIds)
	{
		notice.Public = false;
		foreach (var userId in userIds)
		{
			notice.Receivers!.Add(new Receiver
			{
				UserId = userId
			});
		}

		return await noticesRepository.AddAsync(notice);
	}
	public static async Task<IEnumerable<Notice>> FetchAsync(this IDefaultRepository<Notice> noticesRepository, bool isPublic = true)
		=> await noticesRepository.ListAsync(new NoticesSpecification(isPublic));
	public static NoticeViewModel MapViewModel(this Notice notice, IMapper mapper) 
		=> mapper.Map<NoticeViewModel>(notice);

	public static List<NoticeViewModel> MapViewModelList(this IEnumerable<Notice> notices, IMapper mapper) 
		=> notices.Select(item => MapViewModel(item, mapper)).ToList();

	public static PagedList<Notice, NoticeViewModel> GetPagedList(this IEnumerable<Notice> notices, IMapper mapper, int page = 1, int pageSize = 999)
	{
		var pageList = new PagedList<Notice, NoticeViewModel>(notices, page, pageSize);

		pageList.SetViewList(pageList.List.MapViewModelList(mapper));

		return pageList;
	}

	public static Notice MapEntity(this NoticeViewModel model, IMapper mapper, string currentUserId, Notice? entity = null)
	{
		if (entity == null) entity = mapper.Map<NoticeViewModel, Notice>(model);
		else entity = mapper.Map<NoticeViewModel, Notice>(model, entity);

		if (model.Id == 0) entity.SetCreated(currentUserId);
		else entity.SetUpdated(currentUserId);

		return entity;
	}

	public static IEnumerable<Notice> GetOrdered(this IEnumerable<Notice> notices)
		=> notices.OrderByDescending(item => item.Top).ThenByDescending(item => item.Order)
		.ThenByDescending(item => item.LastUpdated);
	
}
