using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class CategoriesSpecification : Specification<Category>
{
	public CategoriesSpecification()
	{
		Query.Where(item => !item.Removed);
	}

	public CategoriesSpecification(string key)
	{
		Query.Where(item => !item.Removed && item.Key == key);
	}

}
