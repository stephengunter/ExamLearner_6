using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using ApplicationCore.Services;
using ApplicationCore.Helpers;
using ApplicationCore.Hubs;
using Microsoft.AspNetCore.Cors;
using ApplicationCore.Exceptions;

namespace Web.Hubs;

[EnableCors("Api")]
public class NotificationsHub : Hub
{
	private readonly IHubConnectionManager _userConnectionManager;

	public NotificationsHub(IHubConnectionManager userConnectionManager)
	{
		_userConnectionManager = userConnectionManager;
	}

	public string Init()
	{
		var httpContext = this.Context.GetHttpContext();
		var userId = httpContext.Request.Query["user"];
		if (String.IsNullOrEmpty(userId)) throw new KeepUserConnectionFailed("query.user = null");

		_userConnectionManager.KeepUserConnection(userId, Context.ConnectionId);
		return Context.ConnectionId;
	}

	//Called when a connection with the hub is terminated.
	public override Task OnDisconnectedAsync(Exception exception)
	{
		//get the connectionId
		try
		{
			var connectionId = Context.ConnectionId;
			_userConnectionManager.RemoveUserConnection(connectionId);
		}
		catch { }


		return base.OnDisconnectedAsync(exception);
	}
}
