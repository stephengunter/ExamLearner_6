namespace ApplicationCore.Exceptions;

public class EnvironmentVariableNotFound : Exception
{
	public EnvironmentVariableNotFound(string key) : base($"key : {key}")
	{

	}
}
