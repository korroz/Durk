using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Durk.Code;
using Moq;
using SignalR.Hubs;
using System.Dynamic;
using System.Security.Principal;

namespace Durk.Tests.Code
{
	public class ChatTests
	{
		#region Utility

		protected static class Util
		{
		}

		public class TestAgent : IClientAgent
		{
			#region IClientAgent Members

			public System.Threading.Tasks.Task Invoke(string method, params object[] args)
			{
				throw new NotImplementedException();
			}

			#endregion

			public Action<string> addMessageAction;
			public void addMessage(string message)
			{
				if (addMessageAction != null)
					addMessageAction(message);
			}
		}

		#endregion

		[Fact]
		public void Send_Should_Call_addMessage_On_Clients()
		{
			var chat = new Chat();
			var message = "Some line";
			var messageOut = "";
			chat.Agent = new TestAgent { addMessageAction = s => messageOut = s };
			chat.Context = new HubContext(null, null, new GenericPrincipal(new GenericIdentity("SomeUser"), null));

			chat.Send(message);

			Assert.False(messageOut == "");
		}
	}
}
