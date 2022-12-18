namespace ApplicationCore.Views;
public class AnonymousRequest
{
	public AnonymousRequest(string token)
	{
		Token = token;
	}
	public string Token { get; }
}
