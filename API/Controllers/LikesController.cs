using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using API.SignalR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace API.Controllers
{
   public class LikesController(ILikesRepository likesRepository, IHubContext<NotificationsHub> notificationsHubContext) : BaseApiController
   {
      [HttpPost("{targetUserId:int}")]
      public async Task<ActionResult> ToggleLike(int targetUserId)
      {
         var liked = false;

         var sourceUserId = User.GetUserId();
         var sourceUsername = User.GetUsername();

         if (sourceUserId == targetUserId) return BadRequest("You cannot like your own profile");

         var existingLike = await likesRepository.GetUserLike(sourceUserId, targetUserId);

         if (existingLike == null)
         {
            var like = new UserLike
            {
               UserId = sourceUserId,
               LikedUserId = targetUserId,
               CreatedAt = DateTime.UtcNow
            };

            likesRepository.AddLike(like);
            liked = true;
         }
         else
         {
            likesRepository.DeleteLike(existingLike);
         }

         if (await likesRepository.SaveChanges())
         {
            // send notification only if liked and not when disliked
            if (liked)
            {
               var targetUserIdString = targetUserId.ToString();
               // Notify target user
               await notificationsHubContext.Clients
                   .User(targetUserIdString)
                   .SendAsync("ReceiveLikeNotification", new
                   {
                      sourceUserId
                   });
            }


            return Ok();
         }

         return BadRequest("Failed to update like");
      }

      [HttpGet("list")]
      public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
      {
         var likes = await likesRepository.GetCurrentUserLikeIds(User.GetUserId());

         return Ok(likes);
      }

      [HttpGet]
      public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUserLikes([FromQuery] LikeParams likeParams)
      {
         likeParams.UserId = User.GetUserId();
         var users = await likesRepository.GetUserLikes(likeParams);
         Response.AddPaginationHeader(users);

         return Ok(users);
      }

      [HttpGet("latest")]
      public async Task<ActionResult<IEnumerable<LikeNotificationDTO>>> GetLatestLikeNotifications()
      {
         var userId = User.GetUserId();
         var notifications = await likesRepository.GetLatestLikesForUser(userId);
         return Ok(notifications);
      }
   }
}