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
using System.Web.Helpers;
using Durk.Tests.Helpers;

namespace Durk.Tests.Code
{
	public class ChatTests
	{
		[Fact]
		public void Send_Should_Call_addMessage_On_Clients_With_Json_ChatMessage()
		{
			var chat = new Chat();
			var message = "Some line";
			var user = "SomeUser";
			chat.Agent = new TestAgent();
			chat.Context = new HubContext(null, null, new GenericPrincipal(new GenericIdentity(user), null));

			chat.Send(message);

			ChatMessage chatMessage = null;
			Assert.DoesNotThrow(() => chatMessage = ((TestAgent)chat.Agent).CalledMethod("addMessage").GetArgument<string>().FromJsonTo<ChatMessage>());
			Assert.True(chatMessage.Message == message);
			Assert.True(chatMessage.Nick == user);
		}
	}
}
