namespace ApplicationCore.Exceptions;

public class ExamQuestionDuplicated : Exception
{
	public ExamQuestionDuplicated(string message = "") : base(message)
	{

	}
}


public class ExamNotRecruitQuestionSelected : Exception
{

}
