using API.Data;
using API.Data.Repositories;
using API.Interfaces;
using API.Services;
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
      }); // register my DbContext Service
      services.AddCors();
      services.AddScoped<ITokenService, TokenService>();
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); // code to find all the automapper profiles by searching the assemblies
      return services;

   }
}
