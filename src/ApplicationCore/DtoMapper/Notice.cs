using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class NoticeMappingProfile : Profile
{
	public NoticeMappingProfile()
	{
		CreateMap<Notice, NoticeViewModel>();

		CreateMap<NoticeViewModel, Notice>();
	}
}
