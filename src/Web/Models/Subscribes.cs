using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Helpers;
using ApplicationCore.Views;
using Infrastructure.Views;

namespace Web.Models;

public class SubscribesIndexViewModel
{
	public SubscribeViewModel? Current { get; set; }

	public List<BillViewModel> Bills { get; set; } = new List<BillViewModel>();

	public List<SubscribeViewModel> Records { get; set; } = new List<SubscribeViewModel>();

	public PlanViewModel? Plan { get; set; }

	public List<PayWayViewModel> PayWays { get; set; } = new List<PayWayViewModel>();


	public bool CanCreate => Plan != null && PayWays.HasItems();

}

public class SubscribeEditForm
{
	public PlanViewModel? Plan { get; set; }

	public int PayWayId { get; set; }
}

public class BillEditForm
{
	public BillViewModel? Bill { get; set; }

	public PlanViewModel? Plan { get; set; }

	public List<PayWayViewModel> PayWays { get; set; } = new List<PayWayViewModel>();

}
