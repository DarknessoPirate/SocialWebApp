using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace API.Extensions
{
   public static class ClaimsPrincipleExtensions
   {
      public static string GetUsername(this ClaimsPrincipal user)
      {
         var username = user.FindFirstValue(ClaimTypes.Name);

         if (username == null)
            throw new Exception("Cannot get username from token");

         return username;
      }

      public static int GetUserId(this ClaimsPrincipal user)
      {
         var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

         if (userId == null)
            throw new Exception("Cannot get id from token");

         return int.Parse(userId);
      }
   }
}