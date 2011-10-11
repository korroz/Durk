using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Helpers;

namespace Durk.Tests.Helpers
{
   public static class Util
   {
      public static T FromJsonTo<T>(this string json)
      {
         return Json.Decode<T>(json);
      }
   }
}
