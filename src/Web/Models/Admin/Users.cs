using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;
using Infrastructure.Views;
using Microsoft.AspNetCore.Identity;

namespace Web.Models;

public class UsersAdminModel
{
	public ICollection<BaseOption<string>> RolesOptions { get; set; } = new List<BaseOption<string>>();

	public PagedList<User, UserViewModel>? PagedList { get; set; }

}
