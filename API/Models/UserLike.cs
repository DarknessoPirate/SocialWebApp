using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Models
{
   public class UserLike
   {
      public User User { get; set; } = null!;
      public int UserId { get; set; }
      public User LikedUser { get; set; } = null!;
      public int LikedUserId { get; set; }
   }
}