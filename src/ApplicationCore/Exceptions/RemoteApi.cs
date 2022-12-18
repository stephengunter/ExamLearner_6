namespace ApplicationCore.Exceptions;
public class RemoteApiException : Exception
{
	public RemoteApiException(int status, string url) : base($"url: {url} , status: {status}")
	{

	}
	public RemoteApiException(string url, Exception ex) : base(url, ex)
	{

	}
}
