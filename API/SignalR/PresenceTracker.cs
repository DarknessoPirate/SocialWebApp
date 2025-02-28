using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
   public class PresenceTracker
   {
      // Better to replace the dictionary with a DB solution later, this is not a scalable, thread-safe solution
      private static readonly Dictionary<string, List<string>> OnlineUsers = [];

      public Task<bool> UserConnected(string username, string connectionId)
      {
         var isOnline = false;
         // lock resource so nothing else can update it at the same time
         lock (OnlineUsers)
         {
            if (OnlineUsers.ContainsKey(username))
            {
               OnlineUsers[username].Add(connectionId);
            }
            else
            {
               OnlineUsers.Add(username, [connectionId]);
               isOnline = true;
            }
         }

         return Task.FromResult(isOnline);
      }


      public Task<bool> UserDisconnected(string username, string connectionId)
      {
         var isOffline = false;
         lock (OnlineUsers)
         {
            if (!OnlineUsers.ContainsKey(username))
               return Task.FromResult(isOffline);

            OnlineUsers[username].Remove(connectionId);

            if (OnlineUsers[username].Count == 0)
            {
               OnlineUsers.Remove(username);
               isOffline = true;
            }
         }

         return Task.FromResult(isOffline);
      }

      public Task<String[]> GetOnlineUsers()
      {
         string[] onlineUsers;
         lock (OnlineUsers)
         {
            onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
         }

         return Task.FromResult(onlineUsers);
      }

      public static Task<List<string>> GetConnectionsForUser(string username)
      {
         List<string> connectionIds;

         if (OnlineUsers.TryGetValue(username, out var connections))
         {
            lock (connections)
            {
               connectionIds = connections.ToList();
            }
         }
         else
         {
            connectionIds = [];
         }

         return Task.FromResult(connectionIds);
      }
   }
}