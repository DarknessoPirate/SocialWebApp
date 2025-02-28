using System;
using System.Security.Claims;
using API.Data;
using API.DTOs;
using API.Extensions;
using API.Helpers;
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
   public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery] UserParams userParams)
   {
      userParams.CurrentUsername = User.GetUsername();
      var users = await userRepository.GetMembersAsync(userParams);

      Response.AddPaginationHeader(users);

      return Ok(users);
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
   public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
   {

      var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

      if (user == null) return BadRequest("Cannot update user");

      var result = await photoService.AddPhotoAsync(file);

      if (result.Error != null)
         return BadRequest(result.Error.Message);

      var photo = new Photo
      {
         Url = result.SecureUrl.AbsoluteUri,
         PublicId = result.PublicId
      };

      user.Photos.Add(photo);

      if (await userRepository.SaveAllAsync())
         // endpoint name (/api/Users) , the username      ,  the resource to return 
         return CreatedAtAction(nameof(GetUser), new { username = user.UserName }, mapper.Map<PhotoDTO>(photo)); // adds location header to the response

      return BadRequest("Problem adding photo");
   }

   [HttpPut("set-main-photo/{photoId:int}")]
   public async Task<ActionResult> SetMainPhoto(int photoId)
   {
      var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

      if (user == null)
         return BadRequest("Could not find user");

      var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

      if(photo == null || photo.IsMain)
         return BadRequest("Cannot use this photo as main photo");

      var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);

      if (currentMain != null) currentMain.IsMain = false;
      photo.IsMain = true;

      if(await userRepository.SaveAllAsync())
         return NoContent();

      return BadRequest("There was a problem setting the main photo");
   }

   [HttpPut("set-bg-photo/{photoId:int}")]
   public async Task<ActionResult> SetBackgroundPhoto(int photoId)
   {
      var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

      if (user == null)
         return BadRequest("Could not find user");

      var photo = user.Photos.FirstOrDefault(p => p.Id == photoId);

      if(photo == null || photo.IsMain)
         return BadRequest("Cannot use this photo as background photo");

      var currentBackground = user.Photos.FirstOrDefault(p => p.IsBackground);

      if (currentBackground != null) currentBackground.IsBackground = false;
      photo.IsBackground = true;

      if(await userRepository.SaveAllAsync())
         return NoContent();

      return BadRequest("There was a problem setting the main photo");
   }

   [HttpDelete("delete-photo/{photoId:int}")]
   public async Task<ActionResult> DeletePhoto(int photoId){
      var user = await userRepository.GetUserByUsernameAsync(User.GetUsername());

      if(user == null)
         return BadRequest("User not found");

      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

      if(photo == null || photo.IsMain)
         return BadRequest("This photo cannot be deleted. (main photo or photo doesn't exist)");

      if(photo.PublicId == null)
         return BadRequest("Photo PublicId is missing");


      var result = await photoService.DeletePhotoAsync(photo.PublicId);
      if (result.Error != null)
         return BadRequest(result.Error.Message);

      user.Photos.Remove(photo);
      if(await userRepository.SaveAllAsync())
         return Ok();

      return BadRequest("Unable to save changes");

   }
}
