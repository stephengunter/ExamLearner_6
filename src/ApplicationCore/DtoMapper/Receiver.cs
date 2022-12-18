using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ReceiverMappingProfile : Profile
{
	public ReceiverMappingProfile()
	{
		CreateMap<Receiver, ReceiverViewModel>();

		CreateMap<ReceiverViewModel, Receiver>();
	}
}
