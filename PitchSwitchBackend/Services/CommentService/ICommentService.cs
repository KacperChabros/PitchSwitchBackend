using PitchSwitchBackend.Dtos.Comment.Requests;
using PitchSwitchBackend.Dtos.Comment.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.CommentService
{
    public interface ICommentService
    {
        Task<NewCommentDto?> AddComment(AddCommentDto addCommentDto, string userId);
        Task<List<CommentDto>> GetAllComments(CommentQueryObject commentQuery);
        Task<Comment?> GetCommentById(int commentId);
        Task<Comment?> GetCommentByIdWithAllData(int commentId);
        Task<CommentDto?> UpdateComment(Comment comment, UpdateCommentDto updateCommentDto);
        Task DeleteComment(Comment comment);
    }
}
