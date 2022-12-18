using ApplicationCore.Helpers;
using Infrastructure.Views;

namespace ApplicationCore.Views;
public class TermViewModel : BaseCategoryView
{
	public bool ChapterTitle { get; set; }

	public int SubjectId { get; set; }

	public SubjectViewModel? Subject { get; set; }

	public string? Text { get; set; }

	public bool Hide { get; set; }

	public string? QIds { get; set; } //精選試題

	public string? Highlight { get; set; } //json string

	public string? Reference { get; set; } //json string

	public ICollection<string> Highlights { get; set; } = new List<string>();

	public ICollection<ReferenceViewModel> References { get; set; } = new List<ReferenceViewModel>();

	public string FullText => $"{Title} {Text}";

	public ICollection<TermViewModel>? SubItems { get; set; }

	public ICollection<int>? ParentIds { get; set; }

	public ICollection<NoteViewModel>? Notes { get; set; }

	public ICollection<QuestionViewModel>? Questions { get; set; }

	public void LoadNotes(IEnumerable<NoteViewModel> notes)
	{
		Notes = notes.Where(item => item.TermId == this.Id).OrderBy(item => item.Order).ToList();

		foreach (var item in SubItems)
		{
			item.LoadNotes(notes);
		}
	}

	public void LoadQuestions(IEnumerable<QuestionViewModel> questions)
	{
		if (!String.IsNullOrEmpty(QIds))
		{
			var qids = QIds.SplitToIds();

			Questions = questions.Where(item => qids.Contains(item.Id)).ToList();
		}

		foreach (var item in SubItems)
		{
			item.LoadQuestions(questions);
		}
	}
}

public class ReferenceViewModel
{
	public string? Type { get; set; }
	public string? Id { get; set; }
	public string? Text { get; set; }
}
