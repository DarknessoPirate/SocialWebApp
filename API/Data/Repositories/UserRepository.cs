using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repositories
{
   public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
   {
      public async Task<MemberDTO?> GetMemberAsync(string username)
      {
         return await context.Users
            .Where(x => x.Username == username)
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider) // map users into member dto right after selecting
            .SingleOrDefaultAsync();
      }

      public async Task<PageResult<MemberDTO>> GetMembersAsync(UserParams userParams)
      {
         var query = context.Users.AsQueryable();

         // exclude self 
         query = query.Where(x => x.Username != userParams.CurrentUsername);

         // gender filter
         if (userParams.Gender != null)
            query = query.Where(x => x.Gender == userParams.Gender);

         // birthday filter
         var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge - 1));
         var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));
         query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

         query = userParams.OrderBy switch
         {
            "created" => query.OrderByDescending(x => x.Created),
            _ => query.OrderByDescending(x => x.LastActive)
         };

         return await PageResult<MemberDTO>.CreateAsync(query.ProjectTo<MemberDTO>(mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
      }

      public async Task<User?> GetUserByIdAsync(int id)
      {
         return await context.Users.FindAsync(id);
      }

      public async Task<User?> GetUserByUsernameAsync(string username)
      {
         return await context.Users
                  .Include(x => x.Photos)
                  .SingleOrDefaultAsync(user => user.Username == username);
      }

      public async Task<IEnumerable<User>> GetUsersAsync()
      {
         return await context.Users
                  .Include(x => x.Photos)
                  .ToListAsync();
      }

      public async Task<bool> SaveAllAsync()
      {
         return await context.SaveChangesAsync() > 0;
      }

      public void Update(User user)
      {
         context.Entry(user).State = EntityState.Modified;
      }
   }
}