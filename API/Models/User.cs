using System;

namespace API.Models;

public class User
{
    public int Id { get; set; } // entity uses ID as key by default, if it's int i automatically increments their ids when creating new users
    public required string Username {get; set;}

}
