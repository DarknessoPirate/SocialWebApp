using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
   /*
   This is for the development environment only, for the purpose of testing and having some data to work with. 
   */
   public class Seed
   {
      // Asynchronously create some users with data from UserSeedData.json with all fields filled
      // Also add admin/moderator user for the purpose of testing functionality
      public static async Task SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager) // UserManager gives the access to the db
      {

         if (await userManager.Users.AnyAsync())
            return; // return if any users already exist

         var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

         var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

         var users = JsonSerializer.Deserialize<List<User>>(userData, options);

         if (users == null)
            return;

         var roles = new List<Role>
         {
            new Role{Name = "Admin"},
            new Role{Name = "Moderator"},
            new Role{Name = "Member"}
         };

         foreach (var role in roles)
         {
            await roleManager.CreateAsync(role);
         }

         foreach (var user in users)
         {
            await userManager.CreateAsync(user, "Pa$$w0rd"); // creates user and saves it to db
            await userManager.AddToRoleAsync(user, "Member");
         }

         var admin = new User
         {
            UserName = "admin",
            KnownAs = "Admin",
            Gender = "",
            City = "",
            Country = ""
         };

         await userManager.CreateAsync(admin, "Pa$$w0rd");
         await userManager.AddToRolesAsync(admin, ["Admin", "Moderator"]);
      }
   }
}