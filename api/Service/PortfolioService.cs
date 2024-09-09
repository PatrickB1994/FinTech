using System.Net;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;

namespace api.Service
{
    public class PortfolioService : IPortfolioService
    {

        private readonly UserManager<AppUser> _userManager;
        private readonly IStockRepository _stockRepository;
        private readonly IPortfolioRepository _portfolioRepository;
        public PortfolioService(UserManager<AppUser> userManager, IStockRepository stockRepository, IPortfolioRepository portfolioRepository)
        {
            _userManager = userManager;
            _stockRepository = stockRepository;
            _portfolioRepository = portfolioRepository;
        }

        public async Task<List<Stock>> GetUserPortfolio(string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            return await _portfolioRepository.GetUserPortfolio(appUser);
        }

        public async Task<Portfolio> AddPortfolioAsync(string userName, string symbol)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            var stock = await _stockRepository.GetBySymbolAsync(symbol);

            if (stock == null)
            {
                throw new BaseException(HttpStatusCode.NotFound, "Stock not found");
            }

            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);

            if (userPortfolio.Any(s => s.Symbol == symbol))
            {
                throw new BaseException(HttpStatusCode.Found, "Stock already added");
            }

            var portfolioModel = new Portfolio
            {
                StockId = stock.Id,
                AppUserId = appUser.Id
            };

            return await _portfolioRepository.CreateAsync(portfolioModel);
        }

        public async Task<Portfolio> DeletePortfolio(string userName, string symbol)
        {
            var appUser = await _userManager.FindByNameAsync(userName);
            var userPortfolio = await _portfolioRepository.GetUserPortfolio(appUser);
            var filteredStock = userPortfolio.Where(s => s.Symbol.ToLower() == symbol.ToLower()).ToList();
            if (filteredStock.Count == 1)
            {
                return await _portfolioRepository.DeletPortfolio(appUser, symbol);
            }
            else
            {
                throw new BaseException(HttpStatusCode.NotFound, "Stock not in portfolio");
            }
        }
    }
}