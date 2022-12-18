using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class RecruitMappingProfile : Profile
{
	public RecruitMappingProfile()
	{
		CreateMap<Recruit, RecruitViewModel>();

		CreateMap<RecruitViewModel, Recruit>();
	}
}
