using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;

namespace API.DTOs
{
   public class MemberDTO
   {
      public int Id { get; set; } // entity uses ID as key by default, if it's int i automatically increments their ids when creating new users
      public string? Username { get; set; }
      public int Age { get; set; }
      public string? PhotoUrl { get; set; }
      public string? BgUrl {get;set;}
      public string? KnownAs { get; set; }
      public DateTime Created { get; set; } 
      public DateTime LastActive { get; set; } 
      public string? Gender { get; set; }
      public string? Introduction { get; set; }
      public string? Interests { get; set; }
      public string? LookingFor { get; set; }
      public string? City { get; set; }
      public string? Country { get; set; }
      public List<PhotoDTO>? Photos { get; set; } 
   }
}