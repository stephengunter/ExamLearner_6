namespace ApplicationCore.Exceptions;

public class CreateEcPayTradeFailed : Exception
{
	public CreateEcPayTradeFailed(string msg) : base(msg)
	{

	}

	public CreateEcPayTradeFailed(string msg, Exception ex) : base(msg, ex)
	{

	}
}

public class EcPayTradeFeedBackFailed : Exception
{
	public EcPayTradeFeedBackFailed(string msg) : base(msg)
	{

	}
}

public class EcPayTradeFeedBackError : Exception
{
	public EcPayTradeFeedBackError(string msg, Exception ex) : base(msg, ex)
	{

	}

	public EcPayTradeFeedBackError(string msg) : base(msg)
	{

	}
}
