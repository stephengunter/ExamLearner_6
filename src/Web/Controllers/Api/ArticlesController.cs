using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using ApplicationCore.DataAccess;
using ApplicationCore.Services;

namespace Web.Controllers.Api;

public class ArticlesController : BaseApiController
{
	private readonly IDefaultRepository<Article> _articlesRepository;
	private readonly IDefaultRepository<Category> _categoriesRepository;
	private readonly IUsersService _usersService;
	private readonly IMapper _mapper;

	public ArticlesController(IDefaultRepository<Article> articlesRepository, IDefaultRepository<Category> categoriesRepository,
		IUsersService usersService, IMapper mapper)
	{
		_articlesRepository = articlesRepository;
		_categoriesRepository = categoriesRepository;
		_usersService = usersService;
		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(string key)
	{
		if (String.IsNullOrEmpty(key)) return BadRequest();

		var category = await ValidateRequestAsync(key);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var articles = await _articlesRepository.FetchAsync(category!);
		if (articles.HasItems())
		{
			articles = articles.Where(x => x.Active);
			articles = articles.GetOrdered().ToList();
		}

		return Ok(articles.MapViewModelList(_mapper));
	}

	[HttpGet("{id}/{user?}")]
	public async Task<ActionResult> Details(int id, string user = "")
	{
		var article = await _articlesRepository.GetByIdAsync(id);
		if (article == null) return NotFound();

		if (!article.Active)
		{
			var existingUser = await _usersService.FindByIdAsync(user);
			if (existingUser == null) return NotFound();

			bool isAdmin = await _usersService.IsAdminAsync(existingUser);
			if (!isAdmin) return NotFound();
		}

		return Ok(article.MapViewModel(_mapper));
	}

	async Task<Category?> ValidateRequestAsync(string key)
	{
		if (String.IsNullOrEmpty(key))
		{
			ModelState.AddModelError("key", "key不得空白");
			return null;
		}

		var category = await _categoriesRepository.FindByKeyAsync(key);
		if (category == null)
		{
			ModelState.AddModelError("key", "錯誤的key");
			return null;
		}

		return category;

	}

}


