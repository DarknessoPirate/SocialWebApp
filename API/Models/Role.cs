using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Models
{
   public class Role : IdentityRole<int> // use int as id instead of string
   {
      public ICollection<UserRole> UserRoles { get; set; } = [];
   }
}