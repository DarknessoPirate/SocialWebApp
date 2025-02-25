using System;
using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Models;

public class User : IdentityUser<int> // use int as id
{
   // entity uses ID as key by default, if it's int i automatically increments their ids when creating new users
   // IdentityUser has an Id field
   // IdentityUser has a username field
   // IdentityUser has a PasswordHash field

   public DateOnly DateOfBirth { get; set; }
   public required string KnownAs { get; set; }
   public DateTime Created { get; set; } = DateTime.UtcNow;
   public DateTime LastActive { get; set; } = DateTime.UtcNow;
   public required string Gender { get; set; }
   public string? Introduction { get; set; }
   public string? Interests { get; set; }
   public string? LookingFor { get; set; }
   public required string City { get; set; }
   public required string Country { get; set; }
   public List<Photo> Photos { get; set; } = [];
   public List<UserLike> LikedBy { get; set; } = [];
   public List<UserLike> LikedUsers { get; set; } = [];
   public List<Message> MessagesSent { get; set; } = [];
   public List<Message> MessagesReceived { get; set; } = [];
   public ICollection<UserRole> UserRoles { get; set; } = [];

   // Automapper will use Get methods to fill the matching fields in dtos. In this example it will fill Age field thanks to GetAge method
   // public int GetAge()
   // {
   //    return DateOfBirth.CalculateAge();
   // }

}
