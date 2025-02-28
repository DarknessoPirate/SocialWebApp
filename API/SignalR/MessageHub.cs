using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
   public class MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IHubContext<PresenceHub> presenceHub, IMapper mapper) : Hub
   {
      public override async Task OnConnectedAsync()
      {
         var httpContext = Context.GetHttpContext();
         var otherUser = httpContext?.Request.Query["user"];

         if (Context.User == null || string.IsNullOrEmpty(otherUser))
            throw new Exception("Cannot create group");

         var groupName = GetGroupName(Context.User.GetUsername(), otherUser!);
         await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
         var group = await AddToGroup(groupName);

         await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

         var messages = await messageRepository.GetMessageThread(Context.User.GetUsername(), otherUser!);
         await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
      }

      public override async Task OnDisconnectedAsync(Exception? exception)
      {
         var group = await RemoveFromMessageGroup();
         await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
         await base.OnDisconnectedAsync(exception);
      }

      // To invoke in client use matching 'SendMesage' name
      public async Task SendMessage(CreateMessageDTO createMessageDTO)
      {
         var username = Context.User?.GetUsername() ?? throw new Exception("Could not get user");


         if (username == createMessageDTO.RecipientUsername)
            throw new HubException("You cannot message yourself");

         var sender = await userRepository.GetUserByUsernameAsync(username);
         var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

         if (recipient == null || sender == null || sender.UserName == null || recipient.UserName == null)
            throw new HubException("Cannot send message at this time");

         var message = new Message
         {
            Sender = sender,
            SenderUsername = sender.UserName,
            Recipient = recipient,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content
         };

         var groupName = GetGroupName(sender.UserName, recipient.UserName);
         var group = await messageRepository.GetMessageGroup(groupName);

         if (group != null && group.Connections.Any(x => x.Username == recipient.UserName))
         {
            message.DateRead = DateTime.UtcNow;
         }
         else
         {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
            if (connections != null && connections?.Count != null)
            {
               await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                  new {username = sender.UserName, knownAs = sender.KnownAs});
            }
         }

         messageRepository.AddMessage(message);

         if (await messageRepository.SaveAllAsync())
         {
            await Clients.Group(groupName).SendAsync("NewMessage", mapper.Map<MessageDTO>(message));
         }
      }

      // * Store the group with connections in db
      public async Task<Group> AddToGroup(string groupName)
      {
         var username = Context.User?.GetUsername() ?? throw new Exception("Cannot get username");
         var group = await messageRepository.GetMessageGroup(groupName);
         var connection = new Connection { ConnectionId = Context.ConnectionId, Username = username };

         if (group == null)
         {
            group = new Group { Name = groupName };
            messageRepository.AddGroup(group);
         }

         group.Connections.Add(connection);
         if(await messageRepository.SaveAllAsync())
            return group;

         throw new HubException("Failed to join group");
      }

      public async Task<Group> RemoveFromMessageGroup()
      {
         var group = await messageRepository.GetGroupForConnection(Context.ConnectionId);
         var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
         if (connection != null && group != null)
         {
            messageRepository.RemoveConnection(connection);
            if (await messageRepository.SaveAllAsync())
               return group;
         }

         throw new Exception("Failed to remove from group");
      }

      // get alphabetically ordered string of 2 usernames from groupchat, ex: Lisa, Mathew
      private string GetGroupName(string caller, string other)
      {
         var stringCompare = string.CompareOrdinal(caller, other) < 0; // returns <0 if first string is alphabetically less than second, 0 if equal, >1 if second > first
         return stringCompare ? $"{caller},{other}" : $"{other},{caller}";

      }
   }
}