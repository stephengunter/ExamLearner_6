using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class AttachmentMappingProfile : Profile
{
	public AttachmentMappingProfile()
	{
		CreateMap<UploadFile, AttachmentViewModel>();

		CreateMap<AttachmentViewModel, UploadFile>();
	}
}
