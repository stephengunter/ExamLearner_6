using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Views;
public class RefreshTokenRequest
{
	public string AccessToken { get; set; } = String.Empty;
	public string RefreshToken { get; set; } = String.Empty;

}

public class LoginRequest
{
	[Required(ErrorMessage = "必須填寫使用者名稱")]
	public string Username { get; set; } = String.Empty;

	[Required(ErrorMessage = "必須填寫密碼")]
	public string Password { get; set; } = String.Empty;
}

public class OAuthLoginRequest
{
	public string Token { get; set; } = String.Empty;

}
