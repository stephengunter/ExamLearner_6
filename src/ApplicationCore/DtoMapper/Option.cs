using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class OptionMappingProfile : Profile
{
	public OptionMappingProfile()
	{
		CreateMap<Option, OptionViewModel>();

		CreateMap<OptionViewModel, Option>();
	}
}
