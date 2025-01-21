using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{

// Controller used for testing error returns
    public class ErrorController(DataContext context) : BaseApiController
    {
      [Authorize]
      [HttpGet("auth")]
      public ActionResult<string> GetAuth(){
         return "secret";
      }

      
      [HttpGet("not-found")]
      public ActionResult<User> GetNotFound(){
         
         var temp = context.Users.Find(-1);

         if(temp == null) return NotFound();

         return temp;
      }

      
      [HttpGet("server-error")]
      public ActionResult<User> GetServerError(){
         
         var temp = context.Users.Find(-1) ?? throw new Exception("User not found!"); // null dereference casues server error
         
         return temp;
      }

      
      [HttpGet("bad-request")]
      public ActionResult<User> GetBadRequest(){
         
         return BadRequest("Bad request message :)");
      }



    }
}