using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;
using System.Web.Helpers;

namespace Durk.Code
{
	public class Chat : Hub, IDisconnect
	{
		public void Send(string message)
		{
			Clients.addMessage(Json.Encode(new { nick = Context.User.Identity.Name, msg = message }));
		}

		#region IDisconnect Members

		public void Disconnect()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}