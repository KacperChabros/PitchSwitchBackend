using PitchSwitchBackend.Dtos.Comment.Requests;
using PitchSwitchBackend.Dtos.Comment.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Mappers
{
    public static class CommentMapper
    {
        public static Comment FromAddCommentDtoToModel(this AddCommentDto addCommentDto)
        {
            return new Comment
            {
                Content = addCommentDto.Content,
                PostId = addCommentDto.PostId
            };
        }

        public static NewCommentDto FromModelToNewCommentDto(this Comment comment)
        {
            return new NewCommentDto
            {
                CommentId = comment.CommentId,
                Content = comment.Content,
                CreatedOn = comment.CreatedOn,
                IsEdited = comment.IsEdited,
                CreatedByUser = comment.CreatedByUser.FromModelToMinimalUserDto(),
                Post = comment.Post.FromModelToMinimalPostDto()
            };
        }

        public static CommentDto FromModelToCommentDto(this Comment comment)
        {
            return new CommentDto
            {
                CommentId = comment.CommentId,
                Content = comment.Content,
                CreatedOn = comment.CreatedOn,
                IsEdited = comment.IsEdited,
                CreatedByUser = comment.CreatedByUser.FromModelToMinimalUserDto(),
                Post = comment.Post.FromModelToMinimalPostDto()
            };
        }

        public static MinimalCommentDto FromModelToMinimalCommentDto(this Comment comment)
        {
            return new MinimalCommentDto
            {
                CommentId = comment.CommentId,
                Content = comment.Content,
                CreatedOn = comment.CreatedOn,
                IsEdited = comment.IsEdited,
                CreatedByUser = comment.CreatedByUser.FromModelToMinimalUserDto()
            };
        }
    }
}
