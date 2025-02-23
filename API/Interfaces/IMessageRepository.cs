using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Models;

namespace API.Interfaces
{
   public interface IMessageRepository
   {
      void AddMessage(Message message);
      void DeleteMessage(Message message);
      Task<Message?> GetMessage(int id);
      Task<PageResult<MessageDTO>> GetMessagesForUser(MessageParams messageParams);
      Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername);
      Task<bool> SaveAllAsync();
   }
}