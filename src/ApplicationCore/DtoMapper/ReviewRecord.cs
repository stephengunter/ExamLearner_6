using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ReviewRecordMappingProfile : Profile
{
	public ReviewRecordMappingProfile()
	{
		CreateMap<ReviewRecord, ReviewRecordViewModel>();

		CreateMap<ReviewRecordViewModel, ReviewRecord>();
	}
}
