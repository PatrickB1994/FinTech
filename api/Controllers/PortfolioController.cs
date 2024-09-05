using api.Extensions;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Route("api/portfolio")]
    [ApiController]
    [Authorize]
    public class PortfolioController : Controller
    {
        private readonly IPortfolioService _portfolioService;
        public PortfolioController(IPortfolioService portfolioService)
        {
            _portfolioService = portfolioService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserPortfolio()
        {
            var userPortfolio = await _portfolioService.GetUserPortfolio(User.GetUsername());
            var userPortfolioDto = userPortfolio.Select(s => s.ToPortfolioDto());
            return Ok(userPortfolioDto);
        }

        [HttpPost]
        public async Task<IActionResult> AddPortfolio(string symbol)
        {
            var username = User.GetUsername();

            var portfolioModel = await _portfolioService.AddPortfolioAsync(username, symbol);

            if (portfolioModel == null)
            {
                return StatusCode(500, "could not create");
            }
            else
            {
                return Created();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeletePortfolio(string symbol)
        {
            await _portfolioService.DeletePortfolio(User.GetUsername(), symbol);
            return NoContent();
        }
    }
}