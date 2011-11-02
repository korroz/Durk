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
		private ChatHub _product;
		private TestAgent _testAgent = new TestAgent();
		private TestAgent _callerAgent = new TestAgent();
		private GenericPrincipal _principal;
		public ChatHub Object { get { return _product; } }
		public TestAgent Agent { get { return _testAgent; } }
		public TestAgent Caller { get { return _callerAgent; } }

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
		public ChatHub Build()
		{
			_product = new ChatHub();
			_product.Agent = _testAgent;
			_product.Caller = _callerAgent;
			_product.Context = new HubContext(null, null, _principal);
			return _product;
		}
	}
	public class ChatHubTests
	{
		[Fact]
		public void Send_Should_Call_addMessage_On_Clients_With_Json_ChatMessage()
		{
			var message = "Some line";
			var user = "SomeUser";
			var chat = ChatBuilder.New().SetUser(user).Build();

			chat.Send(message);

			ChatMessage chatMessage = ((TestAgent)chat.Agent).CalledMethod("addMessage").GetArgument<string>().FromJsonTo<ChatMessage>();
			Assert.True(chatMessage.Message == message);
			Assert.True(chatMessage.Nick == user);
		}

		[Fact]
		public void Join_Should_Add_User_To_PresentUsers()
		{
			var user = "SomeUser";
			var chat = ChatBuilder.New().SetUser(user).Build();

			chat.Join();

			Assert.Contains(user, chat.PresentUsers);
		}

		[Fact]
		public void Join_Should_Prevent_Duplicate_Users_Present()
		{
			var user = "SomeUser";
			var chat = ChatBuilder.New().SetUser(user).Build();
			chat.PresentUsers.Add(user);

			chat.Join();

			Assert.Single(chat.PresentUsers, user);
		}

		[Fact]
		public void Join_Should_Notify_Clients_When_A_New_User_Joins()
		{
			var user = "SomeUser";
			var chat = ChatBuilder.New().SetUser(user).Build();

			chat.Join();

			var newUser = ((TestAgent)chat.Agent).CalledMethod("userEntered").GetArgument<string>();
			Assert.Equal(user, newUser);
		}

		[Fact]
		public void Join_Should_Not_Notify_Clients_If_An_Old_User_Joins()
		{
			var user = "SomeUser";
			var chat = ChatBuilder.New().SetUser(user).Build();
			chat.PresentUsers.Add(user);

			chat.Join();

			var call = ((TestAgent)chat.Agent).CalledMethod("userEntered");
			Assert.Null(call);
		}

		[Fact]
		public void Join_Should_Provide_Joining_Client_With_A_List_Of_The_Currently_Present_Users()
		{
			var user = "SomeUser";
			var presentUser = "OtherUser";
			var builder = ChatBuilder.New().SetUser(user);
			var chat = builder.Build();
			chat.PresentUsers.Add(presentUser);

			chat.Join();

			var argument = builder.Caller.CalledMethod("presentUsers").GetArgument<string>();
			Assert.Equal("[\"OtherUser\"]", argument);
		}

		[Fact]
		public void Disconnect_Should_Remove_User_From_PresentUsers()
		{
			var user = "SomeUser";
			var chat = ChatBuilder.New().SetUser(user).Build();
			chat.PresentUsers.Add(user);

			chat.Disconnect();

			Assert.DoesNotContain(user, chat.PresentUsers);
		}

		[Fact]
		public void Disconnect_Should_Notify_Clients_When_A_User_Disconnects()
		{
			var user = "SomeUser";
			var chat = ChatBuilder.New().SetUser(user).Build();

			chat.Disconnect();

			var newUser = ((TestAgent)chat.Agent).CalledMethod("userLeft").GetArgument<string>();
			Assert.Equal(user, newUser);
		}
	}
}
