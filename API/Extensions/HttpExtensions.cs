using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Helpers;

namespace API.Extensions
{
   public static class HttpExtensions
   {
      public static void AddPaginationHeader<T>(this HttpResponse response, PageResult<T> data)
      {
         var paginationHeader = new PaginationHeader(data.CurrentPage, data.PageSize, data.TotalCount, data.TotalPages);
         var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
         response.Headers.Append("Pagination", JsonSerializer.Serialize(paginationHeader, jsonOptions)); // this will not be visible to the client app if the line below isnt there
         response.Headers.Append("Access-Control-Expose-Headers", "Pagination"); // make the header visible to client app
      }
   }
}