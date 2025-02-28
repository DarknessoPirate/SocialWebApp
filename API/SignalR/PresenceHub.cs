using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace API.SignalR
{
   [Authorize]
   public class PresenceHub(PresenceTracker presenceTracker) : Hub
   {
      public override async Task OnConnectedAsync()
      {
         var username = Context.User?.GetUsername();
         if (username == null)
            throw new HubException("Cannot get current user claims");

         var isOnline = await presenceTracker.UserConnected(username, Context.ConnectionId);

         if (isOnline)
            await Clients.Others.SendAsync("UserIsOnline", username);

         var currentUsers = await presenceTracker.GetOnlineUsers();
         await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
      }

      public override async Task OnDisconnectedAsync(Exception? exception)
      {
         var username = Context.User?.GetUsername();
         if (username == null)
            throw new HubException("Cannot get current user claims");

         var isOffline = await presenceTracker.UserDisconnected(username, Context.ConnectionId);
         
         if (isOffline)
            await Clients.Others.SendAsync("UserIsOffline", username);

         await base.OnDisconnectedAsync(exception);
      }
   }
}