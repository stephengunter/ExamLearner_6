using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Views;
using ApplicationCore.Paging;
using Infrastructure.Views;
using ApplicationCore.Models;

namespace Web.Models;

public class PaysAdminModel
{
	public PagedList<Pay, PayViewModel>? PagedList { get; set; }

	public ICollection<PayWayViewModel> Payways { get; set; } = new List<PayWayViewModel>();
}
