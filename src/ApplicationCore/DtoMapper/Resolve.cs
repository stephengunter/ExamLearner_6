using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ResolveMappingProfile : Profile
{
	public ResolveMappingProfile()
	{
		CreateMap<Resolve, ResolveViewModel>();

		CreateMap<ResolveViewModel, Resolve>();
	}
}
