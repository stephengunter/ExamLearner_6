using ApplicationCore.Views;

namespace ApplicationCore.Exceptions;
public class BadRequestException : Exception
{
	private readonly ICollection<RequestErrorViewModel> _errors;

	public BadRequestException(ICollection<RequestErrorViewModel> errors)
	{
		_errors = errors;
	}

	public BadRequestException(RequestErrorViewModel error)
	{
		_errors = new List<RequestErrorViewModel> { error };
	}

	public ICollection<RequestErrorViewModel> Errors => _errors;
}
