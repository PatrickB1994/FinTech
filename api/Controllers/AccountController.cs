using api.Dtos.Account;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _accountService.Login(loginDto);
            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = await _accountService.Register(registerDto);
            return Ok(result);
        }
    }
}