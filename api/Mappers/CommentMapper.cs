using api.Dtos.Comment;
using api.Models;

namespace api.Mappers
{
    public static class CommentMapper
    {
        public static CommentDto ToCommentDto(this Comment commentModel)
        {
            return new CommentDto
            {
                Id = commentModel.Id,
                Title = commentModel.Title,
                Content = commentModel.Content,
                CreatedOn = commentModel.CreatedOn,
                CreatedBy = commentModel.AppUser.UserName,
                StockId = commentModel.StockId
            };
        }

        public static Comment ToCommentFromCreateDto(this CreateCommentDto CommentDto, int stockId, string appUserId)
        {
            return new Comment
            {
                Title = CommentDto.Title,
                Content = CommentDto.Content,
                StockId = stockId,
                AppUserId = appUserId
            };
        }

        public static Comment ToCommentFromUpdateDto(this UpdateCommentRequestDto CommentDto)
        {
            return new Comment
            {
                Title = CommentDto.Title,
                Content = CommentDto.Content
            };
        }
    }
}