namespace ApplicationCore.Exceptions;

public class CreateUserException : Exception
{
	public CreateUserException(string message) : base(message)
	{

	}
}

public class UserNotFoundException : Exception
{
	public UserNotFoundException(string val, string key = "Id") : base($"UserNotFound. {key}: {val}")
	{

	}
}

public class AddUserToRoleException : Exception
{
	public AddUserToRoleException(string message) : base(message)
	{

	}
}

public class RemoveUserToRoleException : Exception
{
	public RemoveUserToRoleException(string message) : base(message)
	{

	}
}

public class UserAddPasswordException : Exception
{
	public UserAddPasswordException(string message) : base(message)
	{

	}
}
