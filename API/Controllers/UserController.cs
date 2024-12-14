using System;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController(DataContext context) : ControllerBase
{


    [HttpGet]
    public ActionResult<IEnumerable<User>> GetUsers(){
        var users = context.Users.ToList();

        return Ok(users);
    }

    
    [HttpGet("{id:int}")]
    public ActionResult<User> GetUser(int id){

        var user = context.Users.Find(id);

        if(user == null) 
            return NotFound();

        return Ok(user);
    }
}
