using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;

namespace api.Service
{
    public class StockService : IStockService
    {

        private readonly IStockRepository _stockRepository;
        public StockService(IStockRepository stockRepository)
        {
            _stockRepository = stockRepository;
        }

        public async Task<List<Stock>> GetAll(QueryObject query)
        {
            return await _stockRepository.GetAllAsync(query);
        }

        public async Task<Stock?> GetById(int stockId)
        {
            return await _stockRepository.GetByIdAsync(stockId);
        }

        public async Task<Stock> CreateStock(CreateStockRequestDto createStockDto)
        {
            var stockModel = createStockDto.ToStockfromCreateDto();

            await _stockRepository.CreateAsync(stockModel);

            return stockModel;
        }

        public async Task<Stock?> UpdateStock(int stickId, UpdateStockRequestDto updateStockDto)
        {
            return await _stockRepository.UpdateAsync(stickId, updateStockDto);
        }

        public async Task<Stock?> DeleteStock(int stickId)
        {
            return await _stockRepository.DeleteAsync(stickId);
        }

    }
}