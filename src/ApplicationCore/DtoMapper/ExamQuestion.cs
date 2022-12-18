using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ExamQuestionMappingProfile : Profile
{
	public ExamQuestionMappingProfile()
	{
		CreateMap<ExamQuestion, ExamQuestionViewModel>();

		CreateMap<ExamQuestionViewModel, ExamQuestion>();
	}
}
