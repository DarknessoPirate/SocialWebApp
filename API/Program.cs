using System.Text;
using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Middleware;
using API.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices(builder.Configuration);  // Extension function created for the purpose of delegating the service adding into another function created in extension folder
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddOpenApi(options =>
   options.AddDocumentTransformer<BearerSecuritySchemeTransformer>()
);





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
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient)
           .WithHttpBearerAuthentication(bearer =>
           // temporary short-lived token that will no longer work later
               bearer.Token = "yJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJoYXJyaXMiLCJuYmYiOjE3Mzc2MzgyMTgsImV4cCI6MTczODI0MzAxOCwiaWF0IjoxNzM3NjM4MjE4fQ.53OV5Tv6bG9PTxEww6tA64Jw1B3FYbbxJomVRHcV5ARxc64yvEetkpFulzttXpLi1N-7lb513kKXnkRQ0BKMmQ"
           );
   });
}
//app.UseHttpsRedirection();


// UseCors allows for data fetching from the angular app with origin stated below
app.UseMiddleware<ExceptionMiddleware>();
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200")); // must be declared before MapControllers() to work

app.UseAuthentication(); // before MapControllers, before useAuthorization
app.UseAuthorization(); // after useAuthentication
app.MapControllers();


// Seed the data
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
   var context = services.GetRequiredService<DataContext>();
   await context.Database.MigrateAsync();
   await Seed.SeedUsers(context);

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