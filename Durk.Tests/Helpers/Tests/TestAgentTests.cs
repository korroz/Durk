using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Durk.Tests.Helpers.Tests
{
   public class TestAgentTests
   {
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
   }
}
