namespace ApplicationCore.Exceptions;
public class EmailSendFailed : Exception
{
	public EmailSendFailed(string msg) : base(msg)
	{

	}
}
