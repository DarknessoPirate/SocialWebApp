using System;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs;

public class RegisterDTO
{
    [Required]
    [StringLength(maximumLength:8,MinimumLength = 5)]
    public string Username {get; set;} = String.Empty; // gets rid of the warning
    [Required]
    [StringLength(maximumLength:8,MinimumLength = 5)]
    public string Password {get; set;} = String.Empty;
}
