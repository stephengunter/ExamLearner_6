using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;

namespace Web.Models.Admin;

public class TermEditForm
{
	public TermViewModel Term { get; set; } = new TermViewModel();

	public ICollection<TermViewModel>? Parents { get; set; }
}
