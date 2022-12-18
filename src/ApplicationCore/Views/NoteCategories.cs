namespace ApplicationCore.Views;

public enum NoteCategoryType
{
	Root = 0,
	Subject = 1,
	ChapterTitle = 2
}

public class NoteCategoryViewModel
{
	public int Id { get; set; }
	public int ParentId { get; set; }
	public string? Type { get; set; }
	public string? Text { get; set; }
	public ICollection<NoteCategoryViewModel> SubItems { get; set; } = new List<NoteCategoryViewModel>();
}



