using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class CategoryMappingProfile : Profile
{
	public CategoryMappingProfile()
	{
		CreateMap<Category, CategoryViewModel>();

		CreateMap<CategoryViewModel, Category>();
	}
}
