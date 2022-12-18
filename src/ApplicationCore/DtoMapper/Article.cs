using ApplicationCore.Models;
using ApplicationCore.Views;
using AutoMapper;

namespace ApplicationCore.DtoMapper;

public class ArticleMappingProfile : Profile
{
	public ArticleMappingProfile()
	{
		CreateMap<Article, ArticleViewModel>();

		CreateMap<ArticleViewModel, Article>();
	}
}
