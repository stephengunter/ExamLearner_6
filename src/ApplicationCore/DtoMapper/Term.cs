using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class TermMappingProfile : Profile
{
	public TermMappingProfile()
	{
		CreateMap<Term, TermViewModel>();

		CreateMap<TermViewModel, Term>();
	}
}
