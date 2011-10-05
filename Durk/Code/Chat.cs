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
	public class Chat : Hub, IDisconnect
	{
		public void Send(string message)
		{
			Clients.addMessage(Json.Encode(new ChatMessage { Nick = Context.User.Identity.Name, Message = message }));
		}

		#region IDisconnect Members

		public void Disconnect()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}