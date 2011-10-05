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

namespace Durk.Tests.Code
{
	public static class Util
	{
		public static T FromJsonTo<T>(this string json)
		{
			return Json.Decode<T>(json);
		}
	}

	public class ChatTests
	{
		#region Test helpers

		public class TestAgent : DynamicObject, IClientAgent
		{
			public class CallData
			{
				public List<object[]> CallArguments = new List<object[]>();

				public T GetArgument<T>(int argIndex = 0, int callIndex = 0)
				{
					return (T)CallArguments[callIndex][argIndex];
				}
			}

			private Dictionary<string, CallData> _calls = new Dictionary<string, CallData>();
			#region IClientAgent Members

			public System.Threading.Tasks.Task Invoke(string method, params object[] args)
			{
				throw new NotImplementedException();
			}

			#endregion

			public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
			{
				var calldata = _calls.ContainsKey(binder.Name) ? _calls[binder.Name] : new CallData();
				calldata.CallArguments.Add(args);
				_calls[binder.Name] = calldata;

				result = null;
				return true;
			}

			public bool WasCalled(string methodName)
			{
				return _calls.ContainsKey(methodName);
			}

			public CallData CalledMethod(string methodName)
			{
				return _calls[methodName];
			}
		}

		#endregion

		#region Test for the test helpers
		[Fact]
		public void TestAgent_Should_Save_Info_On_Dynamic_Calls()
		{
			var agent = new TestAgent();

			((dynamic)agent).someMethod();

			Assert.True(agent.WasCalled("someMethod"));
		}

		[Fact]
		public void TestAgent_Should_Provide_Dynamic_Call_Arguments()
		{
			var agent = new TestAgent();

			((dynamic)agent).someMethod(arg: "Some String");

			Assert.True(agent.CalledMethod("someMethod").GetArgument<string>() == "Some String");
		}
		#endregion

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
