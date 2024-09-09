using System.Net;
using api.Dtos.Account;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Service
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        public AccountService(UserManager<AppUser> userManager, ITokenService tokenService, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        public async Task<NewUserDto> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDto.UserName);
            if (user == null)
            {
                throw new HttpException(HttpStatusCode.Unauthorized, "Bad credentials");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                throw new HttpException(HttpStatusCode.Unauthorized, "Bad credentials");
            }
            return new NewUserDto
            {
                UserName = user.UserName,
                Email = user.Email,
                token = _tokenService.CreateToken(user)
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
                        return new NewUserDto
                        {
                            UserName = appUser.UserName,
                            Email = appUser.Email,
                            token = _tokenService.CreateToken(appUser)
                        };
                    }
                    else
                    {
                        throw new HttpException(HttpStatusCode.BadRequest, "Role error: " + roleResult.Errors);
                    }
                }
                else
                {
                    var errorMessage = string.Join(", ", createdUser.Errors.Select(error => error.Description));
                    throw new HttpException(HttpStatusCode.BadRequest, "Create user error: " + errorMessage);
                }
            }
            catch (Exception e)
            {
                throw new HttpException(HttpStatusCode.InternalServerError, e.Message);
            }
        }
    }
}
