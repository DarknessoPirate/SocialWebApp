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
   public class LikesRepository(DataContext context, IMapper mapper) : ILikesRepository
   {
      public void AddLike(UserLike like)
      {
         context.Likes.Add(like);
      }

      public void DeleteLike(UserLike like)
      {
         context.Likes.Remove(like);
      }

      public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
      {
         return await context.Likes.Where(x => x.UserId == currentUserId).Select(x => x.LikedUserId).ToListAsync();
      }

      public async Task<PageResult<MemberDTO>> GetUserLikes(LikeParams likeParams)
      {
         var likes = context.Likes.AsQueryable();
         IQueryable<MemberDTO> query;

         switch (likeParams.Predicate)
         {
            case "liked":
               query = likes.Where(x => x.UserId == likeParams.UserId) // select users liked by the source user
                            .Select(x => x.LikedUser)
                            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
               break;

            case "likedBy":
               query = likes.Where(x => x.LikedUserId == likeParams.UserId)
                            .Select(x => x.User)
                            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
               break;

            default:
               var likeIds = await GetCurrentUserLikeIds(likeParams.UserId);
               query = likes.Where(x => x.LikedUserId == likeParams.UserId && likeIds.Contains(x.UserId))
                            .Select(x => x.User)
                            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider);
               break;
         }
         return await PageResult<MemberDTO>.CreateAsync(query, likeParams.PageNumber, likeParams.PageSize);

      }

      public async Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId)
      {
         return await context.Likes.FindAsync(sourceUserId, targetUserId);
      }

      public async Task<bool> SaveChanges()
      {
         return await context.SaveChangesAsync() > 0;
      }
   }
}