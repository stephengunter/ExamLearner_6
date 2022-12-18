using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ApplicationCore.Helpers;
using ApplicationCore.Exceptions;
using Infrastructure.Entities;

namespace ApplicationCore.Models;
public class ExamQuestion : EntityBase
{
	public int ExamPartId { get; set; }
	public int Order { get; set; }
	public int QuestionId { get; set; }
	public string? AnswerIndexes { get; set; }
	public string? OptionIds { get; set; }// 12,4,34,15
	public string? UserAnswerIndexes { get; set; }
	public bool Correct { get; private set; }



	[Required]
	public virtual ExamPart? ExamPart { get; set; }

	[Required]
	public virtual Question? Question { get; set; }


	[NotMapped]
	public ICollection<Resolve>? Resolves { get; set; }

	[NotMapped]
	public ICollection<Option> Options { get; set; } = new List<Option>();


	#region Helpers

	public void SetCorrect()
	{
		if (AnswerIndexList.IsNullOrEmpty()) throw new NoAnswerToFinishException(ExamPart!.ExamId, this.Id);
		Correct = UserAnswerIndexList!.AllTheSame(AnswerIndexList!);
	}


	public void LoadOptions()
	{
		if (Question!.Options.IsNullOrEmpty()) return;

		this.Options = new List<Option>();
		var ids = OptionIds!.SplitToIds();
		for (int i = 0; i < ids.Count; i++)
		{
			var item = Question.Options.FirstOrDefault(x => x.Id == ids[i]);
			this.Options.Add(item!);
		}

	}

	public void SetAnswerIndexes()
	{
		var answerIndexList = new List<int>();
		var correctOptionIds = Question!.Options.Where(o => o.Correct).Select(o => o.Id);

		for (int i = 0; i < OptionIdsList!.Count; i++)
		{
			if (correctOptionIds.Contains(OptionIdsList[i])) answerIndexList.Add(i);
		}

		AnswerIndexes = answerIndexList.JoinToStringIntegers();

	}

	public void LoadResolves(IEnumerable<Resolve> resolves)
	{
		Resolves = resolves.Where(x => x.QuestionId == QuestionId).HasItems()
							? resolves.Where(x => x.QuestionId == QuestionId).ToList() : new List<Resolve>();

	}


	public List<int>? OptionIdsList => OptionIds!.SplitToIntList();

	public List<int>? AnswerIndexList => AnswerIndexes!.SplitToIntList();

	public List<int>? UserAnswerIndexList => UserAnswerIndexes!.SplitToIntList();

	#endregion


}
