using System;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Extensions;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService) : BaseApiController
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
      

      var user = await userRepository.GetUserByUsernameAsync(User.GetUsername()); // entity framework tracks changes made to this user

      if (user == null)
         return BadRequest("Could not find user");

      mapper.Map(memberUpdateDto, user); // update the fields in user with field from dto by using automapper

      if (await userRepository.SaveAllAsync())  // we can update it like that because entity tracks this user and will update it in db
         return NoContent();

      return BadRequest("Failed to update the user");
   }

   [HttpPost("add-photo")]
   public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file){

      var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

      if (user == null) return BadRequest("Cannot update user");

      var result = await photoService.AddPhotoAsync(file);

      if(result.Error != null)
         return BadRequest(result.Error.Message);

      var photo = new Photo
      {
         Url = result.SecureUrl.AbsoluteUri,
         PublicId = result.PublicId
      };

      user.Photos.Add(photo);

      if (await userRepository.SaveAllAsync())
                            // endpoint name (/api/Users) , the username      ,  the resource to return 
         return CreatedAtAction(nameof(GetUser), new {username = user.Username}, mapper.Map<PhotoDTO>(photo)); // adds location header to the response

      return BadRequest("Problem adding photo");
   }
}
