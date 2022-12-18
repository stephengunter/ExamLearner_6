using ApplicationCore.Views;
using ApplicationCore.Models;
using AutoMapper;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;

namespace ApplicationCore.Helpers;

public static class CategoriesHelpers
{
	public static async Task<IEnumerable<Category>> FetchAsync(this IDefaultRepository<Category> categoriesRepository)
		=> await categoriesRepository.ListAsync(new CategoriesSpecification());
	public static async Task<Category?> FindByKeyAsync(this IDefaultRepository<Category> categoriesRepository, string key)
			=> await categoriesRepository.FirstOrDefaultAsync(new CategoriesSpecification(key));

	public static CategoryViewModel MapViewModel(this Category category, IMapper mapper) 
		=> mapper.Map<CategoryViewModel>(category);

	public static List<CategoryViewModel> MapViewModelList(this IEnumerable<Category> categories, IMapper mapper) 
		=> categories.Select(item => MapViewModel(item, mapper)).ToList();
}
