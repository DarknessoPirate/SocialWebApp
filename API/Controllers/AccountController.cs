using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{

   public class AccountController(DataContext context, ITokenService tokenService, IMapper mapper) : BaseApiController
   {
      [HttpPost("register")]
      public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDto)
      {

         if (await UserExists(registerDto.Username))
            return BadRequest("Username is taken");

         using var hmac = new HMACSHA512();
         var user = mapper.Map<User>(registerDto);
         user.Username = registerDto.Username.ToLower();
         user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
         user.PasswordSalt = hmac.Key;




         context.Users.Add(user);

         return new UserDTO
         {
            Username = user.Username,
            KnownAs = user.KnownAs,
            Token = tokenService.CreateToken(user)
         };
      }

      [HttpPost("login")]
      public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto)
      {
         var user = await context.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(x => x.Username == loginDto.Username.ToLower());

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
            KnownAs = user.KnownAs,
            Token = tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url
         };

      }

      private async Task<bool> UserExists(string username)
      {
         Console.WriteLine($"Checking if '{username}' already exists in database...");
         return await context.Users
            .AsNoTracking()  // Ensure no cache is used
            .AnyAsync(x => x.Username.ToLower() == username);
      }


   }
}
