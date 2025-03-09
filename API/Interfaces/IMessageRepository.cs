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
      void AddGroup(Group group);
      void DeleteMessage(Message message);
      void RemoveConnection(Connection connection);
      Task<Message?> GetMessage(int id);
      Task<Connection?> GetConnection(string connectionId);
      Task<Group?> GetMessageGroup(string groupName);
      Task<Group?> GetGroupForConnection(string connectionId);
      Task<PageResult<MessageDTO>> GetMessagesForUser(MessageParams messageParams);
      Task<PageResult<MessageDTO>> GetLatestMessagesFromUniqueUsers(string username, PaginationParams paginationParams);
      Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string recipientUsername);
      Task<bool> SaveAllAsync();
   }
}