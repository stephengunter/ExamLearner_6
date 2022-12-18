using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace ApplicationCore.DI;
public static class SwaggerDI
{
	public static void AddSwagger(this IServiceCollection services, string title, string version)
	{
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });
			c.EnableAnnotations();
			c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = "Please insert JWT with Bearer into field",
				Name = "Authorization",
				Type = SecuritySchemeType.ApiKey,
			});

			c.AddSecurityRequirement(new OpenApiSecurityRequirement()
			{
				{
					new OpenApiSecurityScheme
					{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Scheme = "oauth2",
							Name = "Bearer",
							In = ParameterLocation.Header,

					},
					new List<string>()
				}
			});
		});
	}
}
