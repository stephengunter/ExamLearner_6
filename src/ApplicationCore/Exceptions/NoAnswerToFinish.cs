namespace ApplicationCore.Exceptions;

public class NoAnswerToFinishException : Exception
{
	public NoAnswerToFinishException(int examId, int examQuestionId) : base($"NoAnswerToFinishException. examId: {examId}  examQuestionId: {examQuestionId}")
	{

	}
}
