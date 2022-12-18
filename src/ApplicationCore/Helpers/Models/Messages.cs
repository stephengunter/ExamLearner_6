using ApplicationCore.Views;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using AutoMapper;
using Newtonsoft.Json;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;

namespace ApplicationCore.Helpers;

public static class MessagesHelpers
{
	public static async Task<IEnumerable<Message>> FetchAsync(this IDefaultRepository<Message> messagesRepository, bool returned)
		=> await messagesRepository.ListAsync(new MessagesSpecification(returned));
	public static MessageViewModel MapViewModel(this Message message, IMapper mapper)
	{
		var model = mapper.Map<MessageViewModel>(message);

		if (message.ReturnContent!.HasValue())
		{
			model.ReturnContentView = JsonConvert.DeserializeObject<BaseMessageViewModel>(message.ReturnContent!);
		}
		else model.ReturnContentView = new BaseMessageViewModel();

		return model;
	}

	public static List<MessageViewModel> MapViewModelList(this IEnumerable<Message> messages, IMapper mapper) => messages.Select(item => MapViewModel(item, mapper)).ToList();

	public static PagedList<Message, MessageViewModel> GetPagedList(this IEnumerable<Message> messages, IMapper mapper, int page = 1, int pageSize = 999)
	{
		var pageList = new PagedList<Message, MessageViewModel>(messages, page, pageSize);

		pageList.SetViewList(pageList.List.MapViewModelList(mapper));

		return pageList;
	}

	public static Message MapEntity(this MessageViewModel model, IMapper mapper, string currentUserId)
	{ 
		var entity = mapper.Map<MessageViewModel, Message>(model);

		entity.Subject = model.Subject.RemoveSciptAndHtmlTags();
		entity.Content = model.Subject.RemoveSciptAndHtmlTags();


		if (model.Id == 0) entity.SetCreated(currentUserId);
		else entity.SetUpdated(currentUserId);

		return entity;
	}

	public static IEnumerable<Message> GetOrdered(this IEnumerable<Message> messages)
		=> messages.OrderByDescending(item => item.CreatedAt);


}
