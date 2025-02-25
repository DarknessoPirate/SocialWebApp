using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

   public class AccountController(UserManager<User> userManager, ITokenService tokenService, IMapper mapper) : BaseApiController
   {
      [HttpPost("register")]
      public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
      {

         if (await UserExists(registerDto.Username))
            return BadRequest("Username is taken");

         var user = mapper.Map<User>(registerDto);
         user.UserName = registerDto.Username;

         var result = await userManager.CreateAsync(user, registerDto.Password);

         if (!result.Succeeded)
            return BadRequest(result.Errors);

         return new UserDTO
         {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = await tokenService.CreateToken(user)
         };
      }

      [HttpPost("login")]
      public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
      {
         var user = await userManager.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(x => x.UserName == loginDto.Username);

         if (user == null || user.UserName == null)
            return Unauthorized("Invalid username");

         var result = await userManager.CheckPasswordAsync(user, loginDto.Password);
         
         if (!result)
            return Unauthorized();


         return new UserDTO
         {
            Username = user.UserName,
            KnownAs = user.KnownAs,
            Token = await tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
         };

      }

      private async Task<bool> UserExists(string username)
      {
         Console.WriteLine($"Checking if '{username}' already exists in database...");
         return await userManager.Users
            .AsNoTracking()  // Ensure no cache is used
            .AnyAsync(x => x.UserName== username);
      }


   }
}
