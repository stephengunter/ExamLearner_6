using ApplicationCore.Helpers;
using System.Security.Claims;
using System.Security.Principal;
using ApplicationCore.Models;

namespace ApplicationCore.Authorization;
public static class ClaimsHelpers
{
	public static IEnumerable<Claim> CreateClaims(this User user, IList<string> roles, OAuth? oAuth = null)
	{
		string id = user.Id;
		string userName = user.UserName;
		var identity = GenerateClaimsIdentity(id, userName);

		var claims = new[]
		{
			identity.FindFirst(JwtClaimIdentifiers.Rol),
			identity.FindFirst(JwtClaimIdentifiers.Id),
			new Claim(JwtClaimIdentifiers.Sub, userName),
			new Claim(JwtClaimIdentifiers.Roles, roles.JoinToString())
		};

		if (oAuth != null)
		{
			claims.Append(new Claim(JwtClaimIdentifiers.Provider, oAuth.Provider.ToString()));
			claims.Append(new Claim(JwtClaimIdentifiers.Picture, oAuth.PictureUrl.GetString()));
			claims.Append(new Claim(JwtClaimIdentifiers.Name, oAuth.GivenName.GetString()));
		}

		return claims!;
	}

	private static ClaimsIdentity GenerateClaimsIdentity(string id, string userName)
	{
		return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
		{
			new Claim(JwtClaimIdentifiers.Id, id),
			new Claim(JwtClaimIdentifiers.Rol, JwtClaims.ApiAccess)
		});
	}

	public static string GetUserId(this ClaimsPrincipal cp) => cp == null ? "" : cp.Claims.First(c => c.Type == "id").Value;

	public static OAuthProvider GetOAuthProvider(this ClaimsPrincipal cp)
	{
		if (cp == null) return OAuthProvider.Unknown;
		string providerName = cp.Claims.First(c => c.Type == JwtClaimIdentifiers.Provider).Value;

		OAuthProvider provider = OAuthProvider.Unknown;
		if (Enum.TryParse(providerName, true, out provider))
		{
			if (Enum.IsDefined(typeof(OAuthProvider), provider)) return provider;
			else return OAuthProvider.Unknown;
		}
		else return OAuthProvider.Unknown;
	}
	public static string UserId(this IEnumerable<Claim> claims)
	{
		var entity = claims.FirstOrDefault(c => c.Type == JwtClaimIdentifiers.Id);
		if (entity == null) return string.Empty;

		return entity.Value;
	}

	public static IEnumerable<string> Roles(this IEnumerable<Claim> claims)
	{
		var entity = claims.FirstOrDefault(c => c.Type == JwtClaimIdentifiers.Roles);
		if (entity == null) return new List<string>();

		return entity.Value.SplitToList();
	}

	public static string UserName(this IEnumerable<Claim> claims)
	{
		var entity = claims.FirstOrDefault(c => c.Type == JwtClaimIdentifiers.Sub);
		if (entity == null) return string.Empty;

		return entity.Value;
	}

	public static bool IsDev(this IEnumerable<Claim> claims)
	{
		var roles = Roles(claims);
		if (roles.IsNullOrEmpty()) return false;

		string devRoleName = AppRoles.Dev.ToString();
		var match = roles.FirstOrDefault(r => r.EqualTo(devRoleName));

		return match != null;
	}
	public static bool IsBoss(this IEnumerable<Claim> claims)
	{
		var roles = Roles(claims);
		if (roles.IsNullOrEmpty()) return false;

		string bossRoleName = AppRoles.Boss.ToString();
		var match = roles.FirstOrDefault(r => r.EqualTo(bossRoleName));

		return match != null;
	}
	public static bool IsSubscriber(this IEnumerable<Claim> claims)
	{
		var roles = Roles(claims);
		if (roles.IsNullOrEmpty()) return false;

		string subscriberRoleName = AppRoles.Subscriber.ToString();
		var match = roles.FirstOrDefault(r => r.EqualTo(subscriberRoleName));

		return match != null;
	}

}
