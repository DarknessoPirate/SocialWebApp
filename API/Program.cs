using System.Text;
using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Middleware;
using API.Models;
using API.Services;
using API.SignalR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


builder.Services.AddApplicationServices(builder.Configuration);  // Extension function created for the purpose of delegating the service adding into another function created in extension folder
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddOpenApi(options =>
   options.AddDocumentTransformer<BearerSecuritySchemeTransformer>()
);



foreach (var key in builder.Configuration.AsEnumerable())
{
    Console.WriteLine($"{key.Key}: {key.Value}");
}

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
   app.MapOpenApi();
   // Scalar used for API testing/documentation
   app.MapScalarApiReference(options =>
   {
      options
           .WithTitle("Darknesso WebAPI")
           .WithTheme(ScalarTheme.DeepSpace)
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
           
   });
}
//app.UseHttpsRedirection();


// UseCors allows for data fetching from the angular app with origin stated below
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowCredentials() // allow credentials to make passing the token to SignalR hubs possible
.WithOrigins("http://localhost:4200", "https://localhost:4200")); // must be declared before MapControllers() to work

app.UseAuthentication(); // before MapControllers, before useAuthorization
app.UseAuthorization(); // after useAuthentication
app.MapControllers();
app.MapHub<PresenceHub>("hubs/presence"); // maps users requests and routes them to the correct hub
app.MapHub<MessageHub>("hubs/message");
app.MapHub<NotificationsHub>("/hubs/notifications");

// Seed the data
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
   var context = services.GetRequiredService<DataContext>();
   await context.Database.MigrateAsync();
   await context.Database.ExecuteSqlRawAsync("DELETE FROM [Connections]"); // remove any existing connections if there are any when restarting
   var userManager = services.GetRequiredService<UserManager<User>>();
   var roleManager = services.GetRequiredService<RoleManager<Role>>();
   await Seed.SeedUsers(userManager, roleManager);

}
catch (Exception ex)
{
   var logger = services.GetRequiredService<ILogger<Program>>();
   logger.LogError(ex, "Error occurred during migration");
}

app.Run();






internal sealed class BearerSecuritySchemeTransformer(IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiDocumentTransformer
{
   public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
   {
      var authSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
      if (authSchemes.Any(authscheme => authscheme.Name == JwtBearerDefaults.AuthenticationScheme))
      {
         var requirements = new Dictionary<string, OpenApiSecurityScheme>
         {
            [JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
            {
               Type = SecuritySchemeType.Http,
               Scheme = JwtBearerDefaults.AuthenticationScheme.ToLower(),
               In = ParameterLocation.Header,
               BearerFormat = "Json Web Token",
            }
         };
         document.Components ??= new OpenApiComponents();
         document.Components.SecuritySchemes = requirements;
      }
   }
}