using api.Dtos.Comment;
using api.Models;

namespace api.Interfaces
{
    public interface ICommentService
    {
        Task<List<Comment>> GetAll();
        Task<Comment?> GetById(int commentId);
        Task<Comment?> CreateComment(int stockId, string userName, CreateCommentDto commentDto);
        Task<Comment?> UpdateComment(int stockId, UpdateCommentRequestDto commentDto);
        Task<Comment?> DeleteComment(int commentId);
    }
}