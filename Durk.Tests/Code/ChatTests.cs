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
	public class ChatBuilder
	{
		private Chat _product = new Chat();
		private TestAgent _testAgent = new TestAgent();
		private GenericPrincipal _principal;
		public Chat Object { get { return _product; } }

		private ChatBuilder() { }
		public static ChatBuilder New()
		{
			return new ChatBuilder();
		}
		public ChatBuilder SetUser(string user)
		{
			_principal = new GenericPrincipal(new GenericIdentity(user), null);
			return this;
		}
		public Chat Build()
		{
			_product.Agent = _testAgent;
			_product.Context = new HubContext(null, null, _principal);
			return _product;
		}
	}
	public class ChatTests
	{
		[Fact]
		public void Send_Should_Call_addMessage_On_Clients_With_Json_ChatMessage()
		{
			var message = "Some line";
			var user = "SomeUser";
			var chat = ChatBuilder.New().SetUser(user).Build();

			chat.Send(message);

			ChatMessage chatMessage = null;
			Assert.DoesNotThrow(() => chatMessage = ((TestAgent)chat.Agent).CalledMethod("addMessage").GetArgument<string>().FromJsonTo<ChatMessage>());
			Assert.True(chatMessage.Message == message);
			Assert.True(chatMessage.Nick == user);
		}
	}
}
