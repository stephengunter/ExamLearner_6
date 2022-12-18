using ApplicationCore.Auth;
using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Settings;
using ApplicationCore.Helpers;
using ApplicationCore.Views;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ApplicationCore.Services;

public interface IAuthService
{
	Task<AuthResponse> CreateTokenAsync(string ipAddress, User user, OAuth oAuth, IList<string>? roles = null);
	Task<AuthResponse> CreateTokenAsync(string ipAddress, User user, IList<string>? roles = null);
	Task CreateUpdateUserOAuthAsync(User user, OAuth oAuth);

	ClaimsPrincipal? ResolveClaimsFromToken(string accessToken);

	Task<bool> IsValidRefreshTokenAsync(string token, User user);

	Task<OAuth?> FindOAuthByProviderAsync(User user, OAuthProvider provider);
}

public class AuthService : IAuthService
{
	private readonly AuthSettings _authSettings;

	private readonly IJwtFactory _jwtFactory;
	private readonly ITokenFactory _tokenFactory;
	private readonly IJwtTokenValidator _jwtTokenValidator;

	private readonly IDefaultRepository<RefreshToken> _refreshTokenRepository;
	private readonly IDefaultRepository<OAuth> _oAuthRepository;

	public AuthService(IOptions<AuthSettings> authSettings, IJwtFactory jwtFactory, ITokenFactory tokenFactory, IJwtTokenValidator jwtTokenValidator,
		IDefaultRepository<RefreshToken> refreshTokenRepository, IDefaultRepository<OAuth> oAuthRepository)
	{
		_authSettings = authSettings.Value;

		_jwtFactory = jwtFactory;
		_tokenFactory = tokenFactory;
		_jwtTokenValidator = jwtTokenValidator;

		_refreshTokenRepository = refreshTokenRepository;
		_oAuthRepository = oAuthRepository;
	}

	int RefreshTokenDaysToExpire => _authSettings.RefreshTokenDaysToExpire < 1 ? 5 : _authSettings.RefreshTokenDaysToExpire;

	string SecretKey => _authSettings.SecurityKey;

	public async Task<AuthResponse> CreateTokenAsync(string ipAddress, User user, OAuth oAuth, IList<string>? roles = null)
	{
		var accessToken = await _jwtFactory.GenerateEncodedTokenAsync(user, roles, oAuth);
		var refreshToken = _tokenFactory.GenerateToken();

		await SetRefreshTokenAsync(ipAddress, user, refreshToken);

		return new AuthResponse(accessToken, refreshToken);
	}

	public async Task<AuthResponse> CreateTokenAsync(string ipAddress, User user, IList<string>? roles = null)
	{
		var accessToken = await _jwtFactory.GenerateEncodedTokenAsync(user, roles);
		var refreshToken = _tokenFactory.GenerateToken();

		await SetRefreshTokenAsync(ipAddress, user, refreshToken);
		return new AuthResponse(accessToken, refreshToken);
	}

	public async Task<OAuth?> FindOAuthByProviderAsync(User user, OAuthProvider provider)
		=> await _oAuthRepository.FindByProviderAsync(user, provider);


	public async Task CreateUpdateUserOAuthAsync(User user, OAuth oAuth)
	{
		var existingEntity = await FindOAuthByProviderAsync(user, oAuth.Provider);

		if (existingEntity != null)
		{
			oAuth.Id = existingEntity.Id;
			_oAuthRepository.DbContext.Entry(existingEntity).CurrentValues.SetValues(oAuth);
			await _oAuthRepository.UpdateAsync(existingEntity);
		}
		else
		{
			await _oAuthRepository.AddAsync(oAuth);
		}

	}

	public ClaimsPrincipal? ResolveClaimsFromToken(string accessToken)
		=> _jwtTokenValidator.GetPrincipalFromToken(accessToken, SecretKey);


	public string GetUserIdFromToken(string accessToken)
	{
		var cp = _jwtTokenValidator.GetPrincipalFromToken(accessToken, SecretKey);
		if (cp == null) return "";

		return cp.Claims.First(c => c.Type == "id").Value;
	}

	public string GetOAuthProviderFromToken(string accessToken)
	{
		var cp = _jwtTokenValidator.GetPrincipalFromToken(accessToken, SecretKey);
		if (cp == null) return "";

		return cp.Claims.First(c => c.Type == "provider").Value;
	}

	public async Task<bool> IsValidRefreshTokenAsync(string token, User user)
	{
		var entity = await GetRefreshTokenAsync(user);
		if (entity == null) return false;

		return entity.Token == token && entity.Active;
	}

	async Task SetRefreshTokenAsync(string ipAddress, User user, string token)
	{
		var expires = DateTime.UtcNow.AddDays(RefreshTokenDaysToExpire);

		var exist = await GetRefreshTokenAsync(user);
		if (exist != null)
		{
			exist.Token = token;
			exist.Expires = expires;
			exist.RemoteIpAddress = ipAddress;

			await _refreshTokenRepository.UpdateAsync(exist);
		}
		else
		{
			var refreshToken = new RefreshToken
			{
				Token = token,
				Expires = expires,
				UserId = user.Id,
				RemoteIpAddress = ipAddress
			};

			await _refreshTokenRepository.AddAsync(refreshToken);

		}

	}

	async Task<RefreshToken?> GetRefreshTokenAsync(User user)
		=> await _refreshTokenRepository.FindAsync(user);

}
