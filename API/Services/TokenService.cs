using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Interfaces;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

// Service used for creating user tokens with data related to their roles, identity, credentials
public class TokenService(IConfiguration config, UserManager<User> userManager) : ITokenService
{
   public async Task<string> CreateToken(User user)
   {
      if (user.UserName == null)
         throw new Exception("No username present for user while creating token");

      var tokenKey = config["TokenKey"] ?? throw new Exception("Cannot access token key from appsettings.json");
      
      if (tokenKey.Length < 64) 
         throw new Exception("Token key needs to be at least 64 characters long");

      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

      
      var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName)
        };

      var roles = await userManager.GetRolesAsync(user);
      claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role))); // add role info to the token

      var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      var tokenDescriptor = new SecurityTokenDescriptor
      {
         Subject = new ClaimsIdentity(claims),
         Expires = DateTime.UtcNow.AddDays(7),
         SigningCredentials = credentials
      };

      var tokenHandler = new JwtSecurityTokenHandler();

      var token = tokenHandler.CreateToken(tokenDescriptor);

      return tokenHandler.WriteToken(token);
   }
}
