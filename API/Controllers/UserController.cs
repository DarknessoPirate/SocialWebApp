using System;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseApiController
{


   [HttpGet]
   public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers()
   {
      var members = await userRepository.GetMembersAsync();

      return Ok(members);
   }



   [HttpGet("{username}")]
   public async Task<ActionResult<MemberDTO>> GetUser(string username)
   {

      var member = await userRepository.GetMemberAsync(username);

      if (member == null)
         return NotFound();

      return Ok(member);
   }

   [HttpPut]
   public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDto)
   {
      var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      if (username == null)
         return BadRequest("No username found in token");

      var user = await userRepository.GetUserByUsernameAsync(username); // entity framework tracks changes made to this user

      if (user == null)
         return BadRequest("Could not find user");

      mapper.Map(memberUpdateDto, user); // update the fields in user with field from dto by using automapper

      if (await userRepository.SaveAllAsync())  // we can update it like that because entity tracks this user and will update it in db
         return NoContent();

      return BadRequest("Failed to update the user");
   }
}
