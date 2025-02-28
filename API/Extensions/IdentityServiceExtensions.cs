using System;
using System.Text;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class IdentityServiceExtensions
{
   public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
   {
      // config identity, - adds role,user and other managers to the service provider
      services.AddIdentityCore<User>(opt =>
      {
         // config the user info complexity here (password complexity for example or email/username)
         opt.Password.RequireNonAlphanumeric = false;

      })
         .AddRoles<Role>()
         .AddRoleManager<RoleManager<Role>>()
         .AddEntityFrameworkStores<DataContext>();


      // set what to validate in token
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
             var tokenKey = config["TokenKey"] ?? throw new Exception("TokenKey not found");
             options.TokenValidationParameters = new TokenValidationParameters
             {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
             };

             // pass the token to signalR on connection ( without it it cant get the token properly)
             options.Events = new JwtBearerEvents
             {
                OnMessageReceived = context =>
                {
                   var accessToken = context.Request.Query["access_token"];
                   var path = context.HttpContext.Request.Path;
                   if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs")) 
                   {
                     context.Token = accessToken;
                   }

                   return Task.CompletedTask;
                }
             };
          });

      // create policies to authenticate users more easily based on rules
      services.AddAuthorizationBuilder()
         .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
         .AddPolicy("ModeratePhotoRole", policy => policy.RequireRole("Admin", "Moderator"));


      return services;
   }
}
