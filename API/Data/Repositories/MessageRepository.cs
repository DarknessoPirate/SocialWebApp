using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories
{
   public class MessageRepository(DataContext context, IMapper mapper) : IMessageRepository
   {
      public void AddMessage(Message message)
      {
         context.Messages.Add(message);
      }

      public void DeleteMessage(Message message)
      {
         context.Messages.Remove(message);
      }

      public async Task<Message?> GetMessage(int id)
      {
         return await context.Messages.FindAsync(id);
      }

      public async Task<PageResult<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
      {
         var query = context.Messages
                              .OrderByDescending(x => x.MessageSent)
                              .AsQueryable();

         query = messageParams.Container switch
         {
            "Inbox" => query.Where(x => x.RecipientUsername == messageParams.Username && x.RecipientDeleted == false),
            "Outbox" => query.Where(x => x.SenderUsername == messageParams.Username && x.SenderDeleted == false),
            _ => query.Where(x => x.RecipientUsername == messageParams.Username && x.DateRead == null && x.RecipientDeleted == false)
         };

         var messages = query.ProjectTo<MessageDTO>(mapper.ConfigurationProvider);

         return await PageResult<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
      }

      // get the message thread between 2 users and order them by time sent, skip deleted messages for user who deleted it
      public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername)
      {
         var messages = await context.Messages
                                       .Include(x => x.Sender) // TODO: optimize this 
                                       .ThenInclude(x => x.Photos)
                                       .Include(x => x.Recipient)
                                       .ThenInclude(x => x.Photos)
                                       .Where(x =>
                                          x.RecipientUsername == currentUsername && x.SenderUsername == recipientUsername && x.RecipientDeleted == false ||
                                          x.RecipientUsername == recipientUsername && x.SenderUsername == currentUsername && x.SenderDeleted == false)
                                       .OrderBy(x => x.MessageSent)
                                       .ToListAsync();

         // get the unread messages sent to current user when he sends the call to get message thread ( they will be set as read once he fetches them )
         var unreadMessages = messages.Where(x => x.DateRead == null && x.RecipientUsername == currentUsername).ToList();

         // mark every unread as read
         if (unreadMessages.Count > 0)
         {
            unreadMessages.ForEach(m => m.DateRead = DateTime.UtcNow);
            await context.SaveChangesAsync();
         }

         // return the thread
         return mapper.Map<IEnumerable<MessageDTO>>(messages);
      }

      public async Task<bool> SaveAllAsync()
      {
         return await context.SaveChangesAsync() > 0;
      }
   }
}