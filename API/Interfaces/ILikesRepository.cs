using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Models;

namespace API.Interfaces
{
   public interface ILikesRepository
   {
      Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId);
      Task<PageResult<MemberDTO>> GetUserLikes(LikeParams likeParams);
      Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);
      void AddLike(UserLike like);
      void DeleteLike(UserLike like);
      Task<bool> SaveChanges();
   }
}