using System.Net;
using api.Data;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace api.Repository
{
    public class PortfolioRepository : IPortfolioRepository
    {
        private readonly ApplicationDBContext _context;
        public PortfolioRepository(ApplicationDBContext applicationDBContext)
        {
            _context = applicationDBContext;
        }

        public async Task<Portfolio> CreateAsync(Portfolio portfolio)
        {
            try
            {
                await _context.Portfolios.AddAsync(portfolio);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException postgresEx && postgresEx.SqlState == "23505")
            {
                // Handle duplicate key violation (unique constraint)
                throw new BaseException(HttpStatusCode.BadRequest, "A portfolio with the same key already exists.");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                throw;
            }
            return portfolio;
        }

        public async Task<Portfolio> DeletPortfolio(AppUser appUser, string symbol)
        {
            var portfolioModel = await _context.Portfolios.FirstOrDefaultAsync(x => x.AppUserId == appUser.Id && x.Stock.Symbol.ToLower() == symbol.ToLower());

            if (portfolioModel == null)
            {
                return null;
            }
            _context.Portfolios.Remove(portfolioModel);
            await _context.SaveChangesAsync();

            return portfolioModel;
        }

        public async Task<List<Stock>> GetUserPortfolio(AppUser user)
        {
            return await _context.Portfolios.Where(u => u.AppUserId == user.Id)
            .Select(stock => new Stock
            {
                Id = stock.StockId,
                Symbol = stock.Stock.Symbol,
                CompanyName = stock.Stock.CompanyName,
                Purchase = stock.Stock.Purchase,
                LastDiv = stock.Stock.LastDiv,
                Industry = stock.Stock.Industry,
                MarketCap = stock.Stock.MarketCap
            }).ToListAsync();
        }
    }
}