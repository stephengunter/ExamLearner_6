using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class NoteMappingProfile : Profile
{
	public NoteMappingProfile()
	{
		CreateMap<Note, NoteViewModel>();

		CreateMap<NoteViewModel, Note>();
	}
}
