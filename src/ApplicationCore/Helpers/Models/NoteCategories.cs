using ApplicationCore.Views;
using ApplicationCore.Models;

namespace ApplicationCore.Helpers;

public static class NoteCategoriesHelpers
{
	public static NoteCategoryViewModel MapNoteCategoryViewModel(this Subject subject, int parentId = 0)
	{
		return new NoteCategoryViewModel
		{
			Id = subject.Id,
			ParentId = parentId,
			Text = subject.Title,
			Type = subject.ParentId > 0 ? NoteCategoryType.Subject.ToString() : NoteCategoryType.Root.ToString()

		};
	}

	public static NoteCategoryViewModel MapNoteCategoryViewModel(this Term term)
	{
		var model = new NoteCategoryViewModel
		{
			Id = term.Id,
			ParentId = term.SubjectId,
			Text = $"{term.Title} {term.Text}",
			Type = NoteCategoryType.ChapterTitle.ToString()

		};

		if (term.SubItems!.HasItems()) model.SubItems = term.SubItems!.Select(item => item.MapNoteCategoryViewModel()).ToList();

		return model;
	}
}
