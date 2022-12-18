using ApplicationCore.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationCore.DI;
public static class DbContextDI
{
	public static void AddDefaultContext(this IServiceCollection services, string connectionString) =>
		  services.AddDbContext<DefaultContext>(options =>
				options.UseSqlServer(connectionString));
}
