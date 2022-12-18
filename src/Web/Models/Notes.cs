using ApplicationCore.Views;

namespace Web.Models;

public class NotesIndexModel
{
	public NotesIndexModel(List<NoteCategoryViewModel> categories, NoteParamsViewModel paramsViewModel)
	{
		Categories = categories;
		Params = paramsViewModel;
	}
	public List<NoteCategoryViewModel> Categories { get; }

	public NoteParamsViewModel Params { get; }
}
