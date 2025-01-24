using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models;

[Table("Photos")]
public class Photo
{
   public int Id { get; set; }
   public required string Url { get; set; }
   public bool IsMain { get; set; }
   public string? PublicId { get; set; }

   // Navigation property []to make the user(owner of photo) non nullable, without it, it allows user id to be null]
   // this bascially sets up required one-to-many relationship
   public int UserId { get; set; }
   public User User = null!; // null forgiving operator
}