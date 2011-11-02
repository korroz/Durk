using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;
using System.Web.Helpers;

namespace Durk.Code
{
	#region Entities
	public class ChatMessage
	{
		public string Nick { get; set; }
		public string Message { get; set; }
	}
	#endregion
	public class ChatHub : Hub, IDisconnect
	{
		private static List<string> _staticUserList = new List<string>();
		public ICollection<string> PresentUsers { get; set; }
		public ChatHub()
		{
			// Poor mans lifetime management, should use a dependency container later.
			// Also detecting the web context shouldn't be this class' concern.
			if (HttpContext.Current == null)
				PresentUsers = new List<string>();
			else
				PresentUsers = _staticUserList;
		}
		public void Send(string message)
		{
			Clients.addMessage(Json.Encode(new ChatMessage { Nick = Context.User.Identity.Name, Message = message }));
		}

		public void Join()
		{
			var user = Context.User.Identity.Name;

			Caller.presentUsers(Json.Encode(PresentUsers));

			if (!PresentUsers.Contains(user))
			{
				PresentUsers.Add(user);
				Clients.userEntered(user);
			}
		}

		#region IDisconnect Members

		public void Disconnect()
		{
			var user = Context.User.Identity.Name;
			PresentUsers.Remove(user);
			Clients.userLeft(user);
		}

		#endregion
	}
}