using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using API.Errors;

namespace API.Middleware
{
   
   // IHostEnvironment used to determine whether app is in production or development mode
   // RequestDelegate is the next middleware to execute, 
   // Logger to log stuff
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
      // Executes when next() called, name has to be exact
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
               await next(context); // Let the middleware chain execute and wait for exceptions to fall back to this middleware
            }
            catch(Exception ex)
            {
               logger.LogError(ex, ex.Message); // log message and exception
               context.Response.ContentType = "application/json"; // set the content type to json 
               context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // set status

               var response = env.IsDevelopment() 
               ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace) // return detailed response in dev mode
               : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server error"); // return short response in production

               var options = new JsonSerializerOptions
               {
                  PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                  WriteIndented = true
               };

               var json = JsonSerializer.Serialize(response, options);

               await context.Response.WriteAsync(json);
            }
        }
    }
}