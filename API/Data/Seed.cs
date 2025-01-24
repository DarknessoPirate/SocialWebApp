using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
   public class Seed
   {
      public static async Task SeedUsers(DataContext context)
      {

         if (await context.Users.AnyAsync())
            return; // return if any users already exist

         var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

         var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

         var users = JsonSerializer.Deserialize<List<User>>(userData, options);

         if (users == null)
            return;

         foreach (var user in users)
         {
            using var hmac = new HMACSHA512();
            user.Username = user.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
            user.PasswordSalt = hmac.Key;

            await context.Users.AddAsync(user);
         }

         await context.SaveChangesAsync();
      }
   }
}