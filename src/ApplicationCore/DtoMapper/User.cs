using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class UserMappingProfile : Profile
{
	public UserMappingProfile()
	{
		CreateMap<User, UserViewModel>();

		CreateMap<UserViewModel, User>();
	}
}
