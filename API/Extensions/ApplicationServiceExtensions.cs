using API.Data;
using API.Data.Repositories;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

// Extend the builder and use this on builder.Services to return all the services to add. 
// This is solely for the purpose of cleaning up the Program.cs file
public static class ApplicationServiceExtensions
{
   public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
   {
      services.AddControllers();
      services.AddDbContext<DataContext>(opt =>
      {
         opt.UseSqlite(config.GetConnectionString("DefaultConnection")); // use Sqlite with default connection string from appsettings.json or appsettings.Development.json depending on mode
      }); // register  DbContext Service

      services.AddCors();
      // Add services for di
      services.AddScoped<ITokenService, TokenService>();
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<ILikesRepository, LikesRepository>();
      services.AddScoped<IMessageRepository, MessageRepository>();
      services.AddScoped<IPhotoService, PhotoService>();
      services.AddScoped<ActivityLogger>();
      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // code to find all the automapper profiles by searching the assemblies
      services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings")); // fill CloudinarySettings class with fields from appsettings
      services.AddSignalR();
      services.AddSingleton<PresenceTracker>();
      return services;

   }
}
