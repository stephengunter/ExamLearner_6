using ApplicationCore.Views;
using Infrastructure.Views;

namespace Web.Models;
public class ManualIndexModel
{
	public List<ManualViewModel> RootItems { get; set; } = new List<ManualViewModel>();

	public List<ManualViewModel> SubItems { get; set; } = new List<ManualViewModel>();
}

public class ManualEditForm
{
	public ManualEditForm(ICollection<BaseOption<int>> parents, ManualViewModel manual)
	{
		Parents = parents;
		Manual = manual;
	}
	public ICollection<BaseOption<int>> Parents { get; }

	public ManualViewModel Manual { get; }
}
