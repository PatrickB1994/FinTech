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