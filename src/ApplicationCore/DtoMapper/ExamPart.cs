using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ExamPartMappingProfile : Profile
{
	public ExamPartMappingProfile()
	{
		CreateMap<ExamPart, ExamPartViewModel>();

		CreateMap<ExamPartViewModel, ExamPart>();
	}
}
