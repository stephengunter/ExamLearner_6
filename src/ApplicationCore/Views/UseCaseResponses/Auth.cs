namespace ApplicationCore.Views;
public class AccessTokenResponse
{
	public string Token { get; }
	public int ExpiresIn { get; }

	public AccessTokenResponse(string token, int expiresIn)
	{
		Token = token;
		ExpiresIn = expiresIn;
	}
}

public class AuthResponse
{
	public AuthResponse(AccessTokenResponse accessToken, string refreshToken)
	{
		AccessToken = accessToken;
		RefreshToken = refreshToken;
	}
	public AccessTokenResponse AccessToken { get; set; }
	public string RefreshToken { get; set; }

}
