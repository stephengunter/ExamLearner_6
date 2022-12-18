using ApplicationCore.Models;
using Ardalis.Specification;

namespace ApplicationCore.Specifications;
public class ArticlesSpecification : Specification<Article>
{
	public ArticlesSpecification()
	{
		Query.Where(item => !item.Removed);
	}
	public ArticlesSpecification(Category category)
	{
		Query.Where(item => !item.Removed && item.CategoryId == category.Id);
	}
}
