using api.Dtos;
using api.Helpers;
using api.Models;

namespace api.Interfaces
{
    public interface IStockService
    {
        Task<List<Stock>> GetAll(QueryObject query);
        Task<Stock?> GetById(int stockId);
        Task<Stock> CreateStock(CreateStockRequestDto createStockDto);
        Task<Stock?> UpdateStock(int stickId, UpdateStockRequestDto updateStockDto);
        Task<Stock?> DeleteStock(int stickId);
    }
}