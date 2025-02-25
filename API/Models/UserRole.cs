using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Models
{
   // join table of users and roles
   public class UserRole : IdentityUserRole<int>
   {
      public User User { get; set; } = null!;
      public Role Role { get; set; } = null!;
   }
}