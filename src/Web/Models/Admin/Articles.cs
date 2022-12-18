using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;

namespace Web.Models.Admin;

public class ArticlesAdminModel
{
	public ICollection<CategoryViewModel>? Categories { get; set; }

	public PagedList<Article, ArticleViewModel>? PagedList { get; set; }
}
