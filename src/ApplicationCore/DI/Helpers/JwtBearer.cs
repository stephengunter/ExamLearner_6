using System.Text;
using Microsoft.Extensions.DependencyInjection;
using ApplicationCore.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ApplicationCore.DI;

public static class JwtBearerDI
{
	public static void AddJwtBearer(this IServiceCollection services, int tokenValidHours, string issuer, string audience, string securityKey)
	{
		var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey));
		services.Configure<JwtIssuerOptions>(options =>
		{
			options.ValidFor = TimeSpan.FromHours(tokenValidHours);
			options.Issuer = issuer;
			options.Audience = audience;
			options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
		});

		JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
		var tokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidIssuer = issuer,

			ValidateAudience = true,
			ValidAudience = audience,

			ValidateIssuerSigningKey = true,
			IssuerSigningKey = signingKey,

			RequireExpirationTime = false,
			ValidateLifetime = true,
			ClockSkew = TimeSpan.Zero
		};

		services.AddAuthentication(options =>
		{
			options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
			options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

		})
		.AddJwtBearer(configureOptions =>
		{
			configureOptions.ClaimsIssuer = issuer;
			configureOptions.TokenValidationParameters = tokenValidationParameters;
			configureOptions.SaveToken = true;

			configureOptions.Events = new JwtBearerEvents
			{
				OnAuthenticationFailed = context =>
				{
					if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
					{
						context.Response.Headers.Add("Token-Expired", "true");
					}
					return Task.CompletedTask;
				}
			};
		});
	}
}
