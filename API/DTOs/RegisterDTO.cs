using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDTO
{
   [Required]
   [StringLength(maximumLength: 10, MinimumLength = 3)]
   public string Username { get; set; } = String.Empty; // gets rid of the warning
   [Required]
   [StringLength(maximumLength: 10, MinimumLength = 6)]
   public string Password { get; set; } = String.Empty;
   [Required]
   public string? KnownAs { get; set; }
   [Required]
   public string? Gender { get; set; }
   [Required]
   public string? DateOfBirth { get; set; }
   [Required]
   public string? City { get; set; }
   [Required]
   public string? Country { get; set; }
}
