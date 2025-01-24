using System;
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
public class UsersController(IUserRepository userRepository) : BaseApiController
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
}
