using Infrastructure.Views;
using Web.Models;
using Microsoft.AspNetCore.Identity;

namespace Web.Helpers;

public static class UsersHelpers
{
	public static void LoadRolesOptions(this UsersAdminModel model, IEnumerable<IdentityRole> roles, string emptyText = "全部")
	{
		var options = roles.Select(x => x.ToOption()).ToList();

		if (!String.IsNullOrEmpty(emptyText)) options.Insert(0, new BaseOption<string>("", emptyText));

		model.RolesOptions = options;
	}

	public static BaseOption<string> ToOption(this IdentityRole role) => new BaseOption<string>(role.Name, role.Name);

}
