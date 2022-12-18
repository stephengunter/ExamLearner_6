using ApplicationCore.Services;
using Microsoft.AspNetCore.Mvc;
using ApplicationCore.Helpers;
using AutoMapper;
using Web.Models;
using Web.Helpers;
using ApplicationCore.Views;

namespace Web.Controllers.Admin;

public class UsersController : BaseAdminController
{
	private readonly IUsersService _usersService;
	private readonly IMapper _mapper;

	public UsersController(IUsersService usersService, IMapper mapper)
	{
		_usersService = usersService;
		_mapper = mapper;
	}

	[HttpGet("")]
	public async Task<ActionResult> Index(string role = "", string keyword = "", int page = 1, int pageSize = 10)
	{
		var model = new UsersAdminModel();

		if (page < 1) //初次載入頁面
		{
			var roles = _usersService.FetchRoles();
			model.LoadRolesOptions(roles);

			page = 1;
		}

		var users = await _usersService.FetchAsync(role);
		if (users.IsNullOrEmpty())
		{
			model.PagedList = users.GetPagedList(_mapper, page, pageSize);
			return Ok(model);
		}

		var keywords = keyword.GetKeywords();
		if (keywords.HasItems()) users = users.FilterByKeyword(keywords);

		var pagedList = users.GetPagedList(_mapper, page, pageSize);

		foreach (var item in pagedList.ViewList)
		{
			var roles = _usersService.GetRolesByUserId(item.Id!);
			item.Roles = String.Join(",", roles.Select(r => r.Name));
		}

		model.PagedList = pagedList;

		return Ok(model);
	}

	[HttpGet("{id}")]
	public async Task<ActionResult> Details(string id)
	{
		var user = await _usersService.FindByIdAsync(id);
		if (user == null) return NotFound();

		var model = user.MapViewModel(_mapper);
		model.HasPassword = await _usersService.HasPasswordAsync(user);

		return Ok(model);
	}


	[HttpPost("password")]
	public async Task<ActionResult> AddPassword([FromBody] SetPasswordRequest model)
	{
		var user = await _usersService.FindByIdAsync(CurrentUserId);
		if (user == null) return NotFound();

		if (!ModelState.IsValid) return BadRequest(ModelState);

		string password = model.Password.Trim();
		if (password.Length < 6)
		{
			ModelState.AddModelError("password", "密碼長度不能小於6個字元");
			return BadRequest(ModelState);
		}

		await _usersService.AddPasswordAsync(user, password);

		return Ok();
	}

}
