using Infrastructure.Views;

namespace ApplicationCore.Views;

public class SubjectViewModel : BaseCategoryView
{
	public ICollection<SubjectViewModel>? SubItems { get; set; }

	public ICollection<int>? SubIds { get; set; }

}
