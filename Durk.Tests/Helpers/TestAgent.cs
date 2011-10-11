using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using SignalR.Hubs;

namespace Durk.Tests.Helpers
{
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
}
