using ApplicationCore.Views;
using ApplicationCore.Models;
using AutoMapper;
using ApplicationCore.Paging;
using ApplicationCore.DataAccess;
using ApplicationCore.Specifications;

namespace ApplicationCore.Helpers;

public static class ArticlesHelpers
{
	public static async Task<IEnumerable<Article>> FetchAsync(this IDefaultRepository<Article> articlesRepository, Category category)
		=> await articlesRepository.ListAsync(new ArticlesSpecification(category));
	public static ArticleViewModel MapViewModel(this Article article, IMapper mapper)
		=> mapper.Map<ArticleViewModel>(article);

	public static List<ArticleViewModel> MapViewModelList(this IEnumerable<Article> articles, IMapper mapper)
		=> articles.Select(item => MapViewModel(item, mapper)).ToList();

	public static PagedList<Article, ArticleViewModel> GetPagedList(this IEnumerable<Article> articles, IMapper mapper, int page = 1, int pageSize = 999)
	{
		var pageList = new PagedList<Article, ArticleViewModel>(articles, page, pageSize);

		pageList.SetViewList(pageList.List.MapViewModelList(mapper));

		return pageList;
	}

	public static Article MapEntity(this ArticleViewModel model, IMapper mapper, string currentUserId, Article? entity = null)
	{
		if (entity == null) entity = mapper.Map<ArticleViewModel, Article>(model);
		else entity = mapper.Map<ArticleViewModel, Article>(model, entity);

		if (model.Id == 0) entity.SetCreated(currentUserId);
		else entity.SetUpdated(currentUserId);

		return entity;
	}

	public static IEnumerable<Article> GetOrdered(this IEnumerable<Article> articles)
		=> articles.OrderBy(item => item.Order).ThenByDescending(item => item.LastUpdated);
}
