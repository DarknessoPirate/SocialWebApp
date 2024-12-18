using System.Text;
using API.Data;
using API.Extensions;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddApplicationServices(builder.Configuration);  // Extension function created for the purpose of delegating the service adding into another function created in extension folder
builder.Services.AddIdentityServices(builder.Configuration);

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Scalar used for API testing/documentation
    app.MapScalarApiReference(options => 
    {
        options
            .WithTitle("My WebAPI")
            .WithTheme(ScalarTheme.DeepSpace)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

//app.UseHttpsRedirection();

//app.UseAuthorization();
// UseCors allows for data fetching from the angular app with origin stated below
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200")); // must be declared before MapControllers() to work

app.UseAuthentication(); // before MapControllers, before useAuthorization
app.UseAuthorization(); // after useAuthentication
app.MapControllers();

app.Run();
