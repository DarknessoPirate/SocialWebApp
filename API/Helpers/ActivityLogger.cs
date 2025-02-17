using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
   public class ActivityLogger : IAsyncActionFilter
   {
      public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
      {
         // execute code before action happens
         var resultContext = await next();
         // execute code after the action happens
         if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

         var userId = resultContext.HttpContext.User.GetUserId();
         var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>(); // get the repository handle
         var user = await repo.GetUserByIdAsync(userId);

         if (user == null) return;

         user.LastActive = DateTime.Now;
         await repo.SaveAllAsync();

      }
   }
}