namespace ApplicationCore.Exceptions;

public class NoActivePlanFound : Exception
{
	public NoActivePlanFound(string message = "") : base(message)
	{

	}
}

public class MutiActivePlanFound : Exception
{
	public MutiActivePlanFound(string message = "") : base(message)
	{

	}
}
