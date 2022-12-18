using Autofac;
using Autofac.Extensions.DependencyInjection;
using ApplicationCore;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using ApplicationCore.DI;
using Microsoft.AspNetCore.Identity;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
bool isDevelopment = builder.Environment.IsDevelopment();

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

// Add services to the container.
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDefaultContext(connectionString);

#region AddIdentity
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
	options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<DefaultContext>()
.AddDefaultTokenProviders();
#endregion

#region Add Configurations
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection(SettingsKeys.AppSettings));
builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection(SettingsKeys.AdminSettings));
#endregion

string clientUrl = builder.Configuration[$"{SettingsKeys.AppSettings}:ClientUrl"];
string adminUrl = builder.Configuration[$"{SettingsKeys.AppSettings}:AdminUrl"];

#region  Add JwtBearer
//string securityKey = builder.Configuration[$"{SettingsKeys.AuthSettings}:SecurityKey"];
//string issuer = builder.Configuration[$"{SettingsKeys.AppSettings}:Name"];
//string audience = clientUrl;
//string tokenValidHours = builder.Configuration[$"{SettingsKeys.AuthSettings}:TokenValidHours"];

//builder.Services.AddJwtBearer(Convert.ToInt32(tokenValidHours), issuer, audience, securityKey);
#endregion

builder.Services.AddDtoMapper();

builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddRazorPages();

if (isDevelopment)
{
	builder.Services.AddSwagger("PosterExamStarter", "v1");
}

builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
	containerBuilder.RegisterModule(new ApplicationCoreModule(isDevelopment));
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (isDevelopment)
{
	app.UseDeveloperExceptionPage();
	// Enable middleware to serve generated Swagger as a JSON endpoint.
	app.UseSwagger();
	// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
	app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PosterExamStarter V1"));
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
	endpoints.MapDefaultControllerRoute();
	endpoints.MapRazorPages();
});

if (isDevelopment)
{
	// Seed Database
	//using (var scope = app.Services.CreateScope())
	//{
	//    var services = scope.ServiceProvider;
	//    try
	//    {
	//        var context = services.GetRequiredService<DefaultContext>();
	//        context.Database.EnsureCreated();

	//        await SeedData.EnsureSeedData(services);
	//    }
	//    catch (Exception ex)
	//    {
	//        var logger = services.GetRequiredService<ILogger<Program>>();
	//        logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
	//    }
	//}

}



app.Run();
