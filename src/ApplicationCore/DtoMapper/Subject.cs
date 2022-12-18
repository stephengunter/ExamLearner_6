using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class SubjectMappingProfile : Profile
{
	public SubjectMappingProfile()
	{
		CreateMap<Subject, SubjectViewModel>();
		CreateMap<SubjectViewModel, Subject>();
	}
}
