using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class QuestionMappingProfile : Profile
{
	public QuestionMappingProfile()
	{
		CreateMap<Question, QuestionViewModel>();

		CreateMap<QuestionViewModel, Question>();
	}
}
