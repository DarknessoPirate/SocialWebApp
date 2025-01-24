using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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

      public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
      {
         return await context.Users
            .ProjectTo<MemberDTO>(mapper.ConfigurationProvider) // map the entities to member dto after select
            .ToListAsync();
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