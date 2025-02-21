using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
   public class LikesController(ILikesRepository likesRepository) : BaseApiController
   {
      [HttpPost("{targetUserId:int}")]
      public async Task<ActionResult> ToggleLike(int targetUserId)
      {
         var sourceUserId = User.GetUserId();

         if (sourceUserId == targetUserId) return BadRequest("You cannot like your own profile");

         var existingLike = await likesRepository.GetUserLike(sourceUserId, targetUserId);

         if (existingLike == null)
         {
            var like = new UserLike
            {
               UserId = sourceUserId,
               LikedUserId = targetUserId
            };

            likesRepository.AddLike(like);
         }
         else
         {
            likesRepository.DeleteLike(existingLike);
         }

         if (await likesRepository.SaveChanges()) return Ok();

         return BadRequest("Failed to update like");
      }

      [HttpGet("list")]
      public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
      {
         var likes = await likesRepository.GetCurrentUserLikeIds(User.GetUserId());

         return Ok(likes);
      }

      [HttpGet]
      public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUserLikes([FromQuery]LikeParams likeParams)
      {
         likeParams.UserId = User.GetUserId();
         var users = await likesRepository.GetUserLikes(likeParams);
         Response.AddPaginationHeader(users);

         return Ok(users);
      }
   }
}