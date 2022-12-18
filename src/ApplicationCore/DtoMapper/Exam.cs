using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ExamMappingProfile : Profile
{
	public ExamMappingProfile()
	{
		CreateMap<Exam, ExamViewModel>();

		CreateMap<ExamViewModel, Exam>();
	}
}
