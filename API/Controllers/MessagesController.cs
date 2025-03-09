using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
   [Authorize]
   public class MessagesController(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper) : BaseApiController
   {
      [HttpPost]
      public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
      {
         var username = User.GetUsername();
         if (username == createMessageDTO.RecipientUsername.ToLower())
            return BadRequest("You can't send a message to yourself");

         var sender = await userRepository.GetUserByUsernameAsync(username);
         var recipient = await userRepository.GetUserByUsernameAsync(createMessageDTO.RecipientUsername);

         if (recipient == null || sender == null || sender.UserName == null || recipient.UserName == null)
            return BadRequest("Cannot send message at this time");

         var message = new Message
         {
            Sender = sender,
            SenderUsername = sender.UserName,
            Recipient = recipient,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content
         };

         messageRepository.AddMessage(message);

         if (await messageRepository.SaveAllAsync())
            return Ok(mapper.Map<MessageDTO>(message));

         return BadRequest("Message failed to send");
      }

      [HttpGet]
      public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessagesForUser([FromQuery] MessageParams messageParams)
      {
         messageParams.Username = User.GetUsername();

         var messages = await messageRepository.GetMessagesForUser(messageParams);
         Response.AddPaginationHeader(messages);
         return messages;
      }

      [HttpGet("thread/{username}")]
      public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
      {
         var currentUsername = User.GetUsername();

         var messageThread = await messageRepository.GetMessageThread(currentUsername, username);
         return Ok(messageThread);
      }

      [HttpGet("latest-messages")]
      public async Task<ActionResult<IEnumerable<MessageDTO>>> GetLatestMessagesFromUniqueUsers([FromQuery] PaginationParams paginationParams)
      {
         var username = User.GetUsername();
         var messages = await messageRepository.GetLatestMessagesFromUniqueUsers(username, paginationParams);

         // Add pagination metadata to response headers
         Response.Headers.Add("Pagination", System.Text.Json.JsonSerializer.Serialize(
             new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages)
         ));

         return Ok(messages);
      }



      [HttpDelete("{id}")]
      public async Task<ActionResult> DeleteMessage(int id)
      {
         var username = User.GetUsername();
         var message = await messageRepository.GetMessage(id);

         if (message == null)
            return BadRequest("Message not found");

         // dont let random users delete somebody's message
         if (message.SenderUsername != username && message.RecipientUsername != username)
            return Forbid();

         if (message.SenderUsername == username)
            message.SenderDeleted = true; // mark as deleted by sender

         if (message.RecipientUsername == username)
            message.RecipientDeleted = true; // mark as deleted by recipient

         // if both users deleted the message - remove it from db and only then
         if (message is { SenderDeleted: true, RecipientDeleted: true })
         {
            messageRepository.DeleteMessage(message);
         }

         if (await messageRepository.SaveAllAsync())
            return Ok();

         return BadRequest("There was a problem deleting the message");
      }

   }
}