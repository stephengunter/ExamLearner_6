using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Entities;
using ApplicationCore.Helpers;

namespace ApplicationCore.Models;
public class Exam : BaseRecord
{
	public ExamType ExamType { get; set; }
	public RecruitExamType RecruitExamType { get; set; } = RecruitExamType.Unknown;
	public OptionType OptionType { get; set; }

	public int RecruitId { get; set; }
	public int SubjectId { get; set; }

	public double Score { get; set; } = -1;
	public bool Reserved { get; set; }
	public string? Title { get; set; }
	public string UserId { get; set; } = String.Empty;

	public virtual ICollection<ExamPart>? Parts { get; set; } = new List<ExamPart>();

	[Required]
	public virtual User? User { get; set; }

	[NotMapped]
	public Subject? Subject { get; private set; }
	public void LoadSubject(IEnumerable<Subject> subjects) => Subject = subjects.FirstOrDefault(x => x.Id == SubjectId);

	#region Helpers

	public void SetAnswers()
	{
		var examQuestions = Parts!.SelectMany(p => p.Questions);
		foreach (var examQuestion in examQuestions)
		{
			examQuestion.SetAnswerIndexes();
		}
	}

	public void Finish()
	{
		double totalScore = 0;
		foreach (var part in Parts!)
		{
			var pointsPerQuestion = part.Points / part.Questions.Count;
			foreach (var examQuestion in part.Questions)
			{
				examQuestion.SetCorrect();
				if (examQuestion.Correct) totalScore += pointsPerQuestion;

			}
		}

		this.Score = totalScore;
	}

	public void LoadPartTitles()
	{
		bool multiParts = Parts!.Count > 1;
		var parts = Parts.ToList();
		for (int i = 0; i < parts.Count; i++)
		{
			var part = parts[i];
			int questionCount = part.Questions.Count;
			var pointsPerQuestion = questionCount > 0 ? (part.Points / questionCount) : 0;
			if (multiParts)
			{
				part.Title = $"第{(i + 1).ToCNNumber()}部份 - 共 {questionCount} 題 每題 {pointsPerQuestion} 分";
			}
			else
			{
				part.Title = $"共 {questionCount} 題 每題{pointsPerQuestion} 分";
			}
		}
	}

	public bool IsComplete => this.Score >= 0;

	public List<int> QuestionIds => Parts!.SelectMany(p => p.Questions).Select(x => x.QuestionId).ToList();

	#endregion



}


public class ExamPart : EntityBase
{
	public int ExamId { get; set; }
	public string? Title { get; set; }
	public int Order { get; set; }
	public int OptionCount { get; set; }
	public double Points { get; set; }
	public bool MultiAnswers { get; set; }
	public virtual ICollection<ExamQuestion> Questions { get; set; } = new List<ExamQuestion>();

	[Required]
	public virtual Exam? Exam { get; set; }

	public void LoadExamQuestions(List<Question> questions)
	{
		Questions = new List<ExamQuestion>();
		for (int i = 0; i < questions.Count; i++)
		{
			var examQuestion = questions[i].ConversionToExamQuestion(OptionCount);
			examQuestion.Order = i + 1;
			Questions.Add(examQuestion);
		}
	}
}

public enum ExamType
{
	Recruit = 0, //歷屆試題
	System = 1, //系統自訂
	Unknown = -1
}

public enum RecruitExamType //歷屆試題測驗模式
{
	Exactly = 0,  //完全相同
	CrossYears = 1,  //各年度交叉
	Unknown = -1
}

public enum OptionType
{
	Number = 0,   // 1,2,3,4
	Alphabet = 1, // A,B,C,D
	Unknown = -1
}
