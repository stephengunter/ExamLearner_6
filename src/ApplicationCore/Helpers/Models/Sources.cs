using ApplicationCore.DataAccess;
using ApplicationCore.Models;
using ApplicationCore.Views;

namespace ApplicationCore.Helpers;

public static class SourcesHelpers
{
	public static async Task MapContentAsync(this SourceViewModel model, IDefaultRepository<Note> notesRepository, IDefaultRepository<Term> termsRepository)
	{
		if (model.NoteId > 0)
		{
			var note = await notesRepository.FindNoteLoadSubItemsAsync(model.NoteId);
			model.MapContent(note!);
		}
		else if (model.TermId > 0)
		{
			var term = await termsRepository.FindTermLoadSubItemsAsync(model.TermId);
			model.MapContent(term!);
		}
	}
	public static void MapContent(this SourceViewModel model, Note note)
	{
		model.TermId = note.TermId;
		model.Title = note.Title;
		model.Text = note.Text;
	}
	public static void MapContent(this SourceViewModel model, Term term)
	{
		model.Title = $"{term.Subject!.Title} {term.Title}";
		model.Text = term.Text;
		model.NoteId = 0;
	}

}
