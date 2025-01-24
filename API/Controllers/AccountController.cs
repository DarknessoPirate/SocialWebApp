using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

   public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
   {
      [Authorize]
      [HttpPost("register")]
      public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
      {

         if (await UserExists(registerDto.Username))
            return BadRequest("Username is taken");

         return Ok();

         // using var hmac = new HMACSHA512();

         // var user = new User
         // {
         //     Username = registerDto.Username.ToLower(),
         //     PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
         //     PasswordSalt = hmac.Key
         // };

         // context.Users.Add(user);
         // await context.SaveChangesAsync();

         // return new UserDTO
         // {
         //     Username = user.Username,
         //     Token = tokenService.CreateToken(user)
         // };
      }

      [HttpPost("login")]
      public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
      {
         var user = await context.Users.FirstOrDefaultAsync(x => x.Username == loginDto.Username.ToLower());

         if (user == null) return Unauthorized("Invalid username");

         using var hmac = new HMACSHA512(user.PasswordSalt);

         var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

         for (int i = 0; i < computedHash.Length; i++)
         {
            if (computedHash[i] != user.PasswordHash[i])
               return Unauthorized("Invalid password");
         }

         return new UserDTO
         {
            Username = user.Username,
            Token = tokenService.CreateToken(user)
         };

      }

      private async Task<bool> UserExists(string username)
      {
         return await context.Users.AnyAsync(x => x.Username.ToLower() == username.ToLower());
      }

   }
}
