using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface ITokenService
    {
        string CreateAccessToken(AppUser appUser);
        string CreateRefreshToken(AppUser appUser);
        bool ValidateRefreshToken(string token);
        string? GetUserName(string token);
    }
}