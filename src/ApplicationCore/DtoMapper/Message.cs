using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class MessageMappingProfile : Profile
{
	public MessageMappingProfile()
	{
		CreateMap<Message, MessageViewModel>();

		CreateMap<MessageViewModel, Message>();
	}
}
