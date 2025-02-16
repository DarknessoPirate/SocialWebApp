using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
   public class PageResult<T> : List<T>
   {
      public int CurrentPage { get; set; }
      public int TotalPages { get; set; }
      public int PageSize { get; set; }
      public int TotalCount { get; set; }

      public PageResult(IEnumerable<T> items, int count, int pageNumber, int pageSize)
      {
         CurrentPage = pageNumber;
         TotalPages = (int)Math.Ceiling(count / (double)pageSize);
         PageSize = pageSize;
         TotalCount = count;
         AddRange(items); // add items to the end of the list
      }

      public static async Task<PageResult<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
      {
         var count = await source.CountAsync();
         var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

         return new PageResult<T>(items,count,pageNumber,pageSize);
      }

   }
}