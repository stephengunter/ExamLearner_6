using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ApplicationCore.Models;
using ApplicationCore.Helpers;

namespace ApplicationCore.DataAccess;

public static class SeedData
{
	static string AdminEmail = "traders.com.tw@gmail.com";
	static string SubscriberRoleName = AppRoles.Subscriber.ToString();
	static string DevRoleName = AppRoles.Dev.ToString();
	static string BossRoleName = AppRoles.Boss.ToString();
	public static async Task EnsureSeedData(IServiceProvider serviceProvider)
    {
		Console.WriteLine("Seeding database...");
		//using (var defaultContext = serviceProvider.GetRequiredService<DefaultContext>())
		//{
			


		//}
		using (var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>())
		{
			await SeedRoles(roleManager);
		}
        using (var userManager = serviceProvider.GetRequiredService<UserManager<User>>())
        {
            await SeedUsers(userManager);
        }

        Console.WriteLine("Done seeding database.");
		Console.WriteLine();
	}

	static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
	{
		var roles = new List<string> { DevRoleName, BossRoleName, SubscriberRoleName };
		foreach (var item in roles) await AddRoleIfNotExist(roleManager, item);
	}
	static async Task AddRoleIfNotExist(RoleManager<IdentityRole> roleManager, string roleName)
	{
		var role = await roleManager.FindByNameAsync(roleName);
		if (role == null) await roleManager.CreateAsync(new IdentityRole { Name = roleName });
	}
	static async Task SeedUsers(UserManager<User> userManager)
	{
		string email = AdminEmail;
		var roles = new List<string>() { DevRoleName };

		await CreateUserIfNotExist(userManager, email, roles);

	}
	static async Task CreateUserIfNotExist(UserManager<User> userManager, string email, IList<string>? roles = null)
	{
		var user = await userManager.FindByEmailAsync(email);
		if (user == null)
		{
			bool isAdmin = false;
			if (roles!.HasItems())
			{
				isAdmin = roles!.Select(r => r.EqualTo(DevRoleName) || r.EqualTo(BossRoleName)).FirstOrDefault();
			}

			var newUser = new User
			{
				Email = email,
				UserName = email,
				EmailConfirmed = isAdmin,
				SecurityStamp = Guid.NewGuid().ToString()
			};


			var result = await userManager.CreateAsync(newUser);

			if (!roles.IsNullOrEmpty())
			{
				await userManager.AddToRolesAsync(newUser, roles);
			}


		}
		else
		{
			if (!roles.IsNullOrEmpty())
			{
				foreach (var role in roles!)
				{
					bool hasRole = await userManager.IsInRoleAsync(user, role);
					if (!hasRole) await userManager.AddToRoleAsync(user, role);
				}
			}

		}
	}
}
