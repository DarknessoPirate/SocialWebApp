using System;
using API.Models;

namespace API.Interfaces;

public interface ITokenService
{
   Task<string> CreateToken(User user);
}
