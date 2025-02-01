using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Models;
using AutoMapper;
using AutoMapper.Execution;

namespace API.Helpers
{
   public class AutoMapper : Profile
   {
      public AutoMapper()
      {
         CreateMap<User, MemberDTO>()
            .ForMember(x => x.Age, o => o.MapFrom(s => s.DateOfBirth.CalculateAge()))
            .ForMember(x => x.PhotoUrl, options => options.MapFrom(source => source.Photos.FirstOrDefault(x => x.IsMain)!.Url)); // automapper will set photoUrl to null if no photo found, thus ! null forgiving operator here

         CreateMap<Photo, PhotoDTO>();

         CreateMap<MemberUpdateDTO,User>();
      }
   }
}