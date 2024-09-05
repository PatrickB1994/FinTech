using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Models;

namespace api.Interfaces
{
    public interface IPortfolioService
    {
        Task<List<Stock>> GetUserPortfolio(string userName);
        Task<Portfolio> AddPortfolioAsync(string userName, string symbol);
        Task<Portfolio> DeletePortfolio(string userName, string symbol);
    }
}