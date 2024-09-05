using api.Dtos.Comment;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Identity;

namespace api.Service
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IStockRepository _stockRepository;
        private readonly UserManager<AppUser> _userManager;

        public CommentService(ICommentRepository commentRepository, IStockRepository stockRepository, UserManager<AppUser> userManager)
        {
            _commentRepository = commentRepository;
            _stockRepository = stockRepository;
            _userManager = userManager;
        }

        public async Task<List<Comment>> GetAll()
        {
            // IEnumerable<CommentDto> commentDtos = 
            // from comment in comments
            // where comment.Id != 0
            // select comment.ToCommentDto();

            return await _commentRepository.GetAllAsync();
        }

        public async Task<Comment?> GetById(int commentId)
        {
            return await _commentRepository.GetByIdAsync(commentId);
        }

        public async Task<Comment?> CreateComment(int stockId, string userName, CreateCommentDto commentDto)
        {
            if (!await _stockRepository.StockExists(stockId))
            {
                return null;
            }
            var appUser = await _userManager.FindByNameAsync(userName);
            var commentModel = commentDto.ToCommentFromCreateDto(stockId, appUser.Id);
            await _commentRepository.CreateAsync(commentModel);
            return commentModel;
        }

        public async Task<Comment?> UpdateComment(int stockId, UpdateCommentRequestDto commentDto)
        {
            return await _commentRepository.UpdateAsync(stockId, commentDto.ToCommentFromUpdateDto());
        }

        public async Task<Comment?> DeleteComment(int commentId)
        {
            return await _commentRepository.DeleteAsync(commentId);
        }
    }
}