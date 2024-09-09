using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Interfaces;
using api.Models;
using Microsoft.IdentityModel.Tokens;

namespace api.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;
        private readonly SymmetricSecurityKey _refreshKey;
        private readonly int _accessDuration;
        private readonly int _refreshDuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SigningKey"]));
            _refreshKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:RefreshSigningKey"]));
            _accessDuration = _configuration.GetValue<int>("JWT:AccessDuration");
            _refreshDuration = _configuration.GetValue<int>("JWT:RefreshDuration");
        }

        public string CreateAccessToken(AppUser appUser)
        {
            return CreateToken(appUser, _accessDuration, _key);
        }

        public string CreateRefreshToken(AppUser appUser)
        {
            return CreateToken(appUser, _refreshDuration, _refreshKey);
        }

        private string CreateToken(AppUser appUser, int minutesToExpire, SymmetricSecurityKey signingKey)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, appUser.UserName)
            };

            var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(minutesToExpire),
                SigningCredentials = creds,
                Issuer = _configuration["JWT:Issuer"],
                Audience = _configuration["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string? GetUserName(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var claims = jwtToken.Claims;
            var userName = claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.GivenName)?.Value;
            return userName;
        }

        public bool ValidateRefreshToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["JWT:Issuer"],
                    ValidAudience = _configuration["JWT:Audience"],
                    IssuerSigningKey = _refreshKey
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (SecurityTokenExpiredException)
            {
                // Handle the case where the token has expired
                return false;
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                // Handle invalid signature
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}