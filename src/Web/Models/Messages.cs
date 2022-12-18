using System;
using System.Collections.Generic;
using System.Text;
using ApplicationCore.Models;
using ApplicationCore.Paging;
using ApplicationCore.Views;
using Infrastructure.Views;


namespace Web.Models;

public class MessageEditForm : AnonymousRequest
{
	public MessageEditForm(string token, MessageViewModel message) : base(token)
	{
		Message = message;
	}
	public MessageViewModel Message { get; }
}
