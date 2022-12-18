using ApplicationCore.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Web.Models.Admin;
using ApplicationCore.Helpers;
using ApplicationCore.Views;
using ApplicationCore.DataAccess;

namespace Web.Controllers.Admin;
public class ArticlesController : BaseAdminController
{
	private readonly IDefaultRepository<Article> _articleRepository;
	private readonly IDefaultRepository<Category> _categoryRepository;
	private readonly IMapper _mapper;

	public ArticlesController(IDefaultRepository<Article> articleRepository, IDefaultRepository<Category> categoryRepository, IMapper mapper)
	{
		_articleRepository = articleRepository;
		_categoryRepository = categoryRepository;
		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(int category, int active, int page = 1, int pageSize = 10)
	{
		var model = new ArticlesAdminModel();
		if (category < 1)
		{
			var categories = await _categoryRepository.FetchAsync();
			model.Categories = categories.MapViewModelList(_mapper);

			category = model.Categories.FirstOrDefault()!.Id;
		}

		var articles = await _articleRepository.FetchAsync(new Category { Id = category });
		if (articles.HasItems())
		{
			articles = articles.Where(x => x.Active == active.ToBoolean()).ToList();
			articles = articles.GetOrdered().ToList();
		}

		model.PagedList = articles.GetPagedList(_mapper, page, pageSize);

		return Ok(model);
	}

	[HttpGet("create")]
	public ActionResult Create()
	{
		return Ok(new ArticleViewModel() { Active = false, Order = -1 });
	}

	[HttpPost("")]
	public async Task<ActionResult> Store([FromBody] ArticleViewModel model)
	{
		ValidateRequest(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		var article = model.MapEntity(_mapper, CurrentUserId);
		article.Order = model.Active ? 0 : -1;

		article = await _articleRepository.AddAsync(article);

		return Ok(article.MapViewModel(_mapper));
	}
	[HttpGet("edit/{id}")]
	public async Task<ActionResult> Edit(int id)
	{
		var article = await _articleRepository.GetByIdAsync(id);
		if (article == null) return NotFound();

		var model = article.MapViewModel(_mapper);

		return Ok(model);
	}

	[HttpPut("{id}")]
	public async Task<ActionResult> Update(int id, [FromBody] ArticleViewModel model)
	{
		var article = await _articleRepository.GetByIdAsync(id);
		if (article == null) return NotFound();

		ValidateRequest(model);
		if (!ModelState.IsValid) return BadRequest(ModelState);

		article = model.MapEntity(_mapper, CurrentUserId, article);
		article.Order = model.Active ? 0 : -1;

		await _articleRepository.UpdateAsync(article);

		return Ok(article.MapViewModel(_mapper));
	}

	[HttpPost("off")]
	public async Task<ActionResult> Off([FromBody] ArticleViewModel model)
	{
		var article = await _articleRepository.GetByIdAsync(model.Id);
		if (article == null) return NotFound();

		article.Order = -1;
		await _articleRepository.UpdateAsync(article);

		return Ok();
	}

	[HttpDelete("{id}")]
	public async Task<ActionResult> Delete(int id)
	{
		var article = await _articleRepository.GetByIdAsync(id);
		if (article == null) return NotFound();

		article.Order = -1;
		article.Removed = true;
		article.SetUpdated(CurrentUserId);

		await _articleRepository.UpdateAsync(article);

		return Ok();
	}


	void ValidateRequest(ArticleViewModel model)
	{
		if (String.IsNullOrEmpty(model.Title)) ModelState.AddModelError("title", "必須填寫標題");

		if (String.IsNullOrEmpty(model.Content)) ModelState.AddModelError("content", "必須填寫內容");

	}
}
