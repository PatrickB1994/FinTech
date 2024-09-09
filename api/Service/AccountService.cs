using System.Net;
using api.Dtos.Account;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Service
{
    [LogAspect]
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly string _loginProvider;
        public AccountService(ILogger<AccountService> logger, IConfiguration configuration, UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _configuration = configuration;
            _loginProvider = _configuration["JWT:LoginProvider"];
        }

        public async Task<NewUserDto> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName);
            if (user == null)
            {
                _logger.LogError("Bad credentials for user: {}", loginDto.UserName);
                throw new BaseException(HttpStatusCode.Unauthorized, "Bad credentials");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                _logger.LogError("Bad credentials for user: {}", loginDto.UserName);
                throw new BaseException(HttpStatusCode.Unauthorized, "Bad credentials");
            }

            var refreshToken = _tokenService.CreateRefreshToken(user);
            await _userManager.SetAuthenticationTokenAsync(user, _loginProvider, "RefreshToken", refreshToken);

            return new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateAccessToken(user),
                RefreshToken = refreshToken
            };
        }

        public async Task<NewUserDto> Register(RegisterDto registerDto)
        {
            try
            {
                var appUser = new AppUser
                {
                    UserName = registerDto.UserName,
                    Email = registerDto.Email,
                };
                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        var refreshToken = _tokenService.CreateRefreshToken(appUser);
                        await _userManager.SetAuthenticationTokenAsync(appUser, _loginProvider, "RefreshToken", refreshToken);

                        return new NewUserDto
                        {
                            UserName = appUser.UserName,
                            Email = appUser.Email,
                            Token = _tokenService.CreateAccessToken(appUser),
                            RefreshToken = refreshToken
                        };
                    }
                    else
                    {
                        _logger.LogError("Role error: {}" + roleResult.Errors);
                        throw new BaseException(HttpStatusCode.BadRequest, "Role error: " + roleResult.Errors);
                    }
                }
                else
                {
                    var errorMessage = string.Join(", ", createdUser.Errors.Select(error => error.Description));
                    _logger.LogError("Create user error: {}" + errorMessage);
                    throw new BaseException(HttpStatusCode.BadRequest, "Create user error: " + errorMessage);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Create user exception: {}" + e.Message);
                throw new BaseException(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        public async Task<NewUserDto> RefreshToken(RefreshTokenDto refreshDto)
        {
            var _refreshToken = refreshDto.RefreshToken;

            if (!_tokenService.ValidateRefreshToken(_refreshToken))
            {
                throw new BaseException(HttpStatusCode.Unauthorized, "Invalid token");
            }

            var userName = _tokenService.GetUserName(_refreshToken);
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            var cachedToken = await _userManager.GetAuthenticationTokenAsync(user, _loginProvider, "RefreshToken");

            if (cachedToken == null || !cachedToken.Equals(_refreshToken))
            {
                _logger.LogError("Invalid token not in DB");
                throw new BaseException(HttpStatusCode.Unauthorized, "Invalid token");
            }

            await _userManager.RemoveAuthenticationTokenAsync(user, _loginProvider, _refreshToken);
            var _newRefreshToken = _tokenService.CreateRefreshToken(user);
            await _userManager.SetAuthenticationTokenAsync(user, _loginProvider, "RefreshToken", _newRefreshToken);

            return new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                Token = _tokenService.CreateAccessToken(user),
                RefreshToken = _newRefreshToken
            };
        }
    }
}
