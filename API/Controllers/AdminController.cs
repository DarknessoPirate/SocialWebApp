using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

   public class AdminController(UserManager<User> userManager) : BaseApiController
   {
      [Authorize(Policy = "RequireAdminRole")]
      [HttpGet("get-users-with-roles")]
      public async Task<ActionResult> GetUsersWithRoles()
      {
         var users = await userManager.Users
            .OrderBy(x => x.UserName)
            .Select(x => new
            {
               x.Id,
               Username = x.UserName,
               Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

         return Ok(users);
      }

      /* 
         Endpoint to edit the user's roles (pass the selected roles the user should have, the rest will get removed)
      */
      [Authorize(Policy = "RequireAdminRole")]
      [HttpPost("edit-roles/{username}")]
      public async Task<ActionResult> EditRoles(string username, string roles)
      {
         if(string.IsNullOrEmpty(roles))
            return BadRequest("You must select at least one role");
         
         var selectedRoles = roles.Split(",").ToArray(); // parse the roles string into an array of role string
         var user = await userManager.FindByNameAsync(username);

         if (user == null)
            return BadRequest("User not found");

         var userRoles = await userManager.GetRolesAsync(user);
         var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles)); // add wanted roles

         if(!result.Succeeded)
            return BadRequest("Failed to add roles");

         // remove unwanted roles
         result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
         if(!result.Succeeded)
            return BadRequest("Failed to remove unwanted roles");

         // return the updated list of roles
         return Ok(await userManager.GetRolesAsync(user));
      }

      [Authorize(Policy = "ModeratePhotoRole")]
      [HttpGet("get-photos-for-moderation")]
      public ActionResult GetPhotosForModeration()
      {
         return Ok("Admins or moderators can see this");
      }
   }


}