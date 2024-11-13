using PitchSwitchBackend.Dtos.Post.Requests;
using PitchSwitchBackend.Dtos.Post.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Mappers
{
    public static class PostMapper
    {
        public static NewPostDto FromModelToNewPostDto(this Post post)
        {
            return new NewPostDto
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedOn = post.CreatedOn,
                CreatedByUser = post.CreatedByUser.FromModelToMinimalUserDto(),
                Transfer = post.Transfer?.FromModelToMinimalTransferDto(),
                TransferRumour = post.TransferRumour?.FromModelToMinimalTransferRumourDto()
            };
        }
        public static PostDto FromModelToPostDto(this Post post)
        {
            return new PostDto
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedOn = post.CreatedOn,
                IsEdited = post.IsEdited,
                CreatedByUser = post.CreatedByUser.FromModelToMinimalUserDto(),
                Transfer = post.Transfer?.FromModelToMinimalTransferDto(),
                TransferRumour = post.TransferRumour?.FromModelToMinimalTransferRumourDto(),
                Comments = post.Comments.Select(c => c.FromModelToMinimalCommentDto()).ToList()
            };
        }

        public static Post FromAddPostDtoToModel(this AddPostDto addPostDto)
        {
            return new Post
            {
                Title = addPostDto.Title,
                Content = addPostDto.Content,
                TransferId = addPostDto.TransferId,
                TransferRumourId = addPostDto.TransferRumourId
            };
        }

        public static MinimalPostDto FromModelToMinimalPostDto(this Post post)
        {
            return new MinimalPostDto
            {
                PostId = post.PostId,
                Title = post.Title,
                CreatedByUser = post.CreatedByUser.FromModelToMinimalUserDto()
            };
        }

        public static ListElementPostDto FromModelToListElementPostDto(this Post post)
        {
            return new ListElementPostDto
            {
                PostId = post.PostId,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                CreatedOn = post.CreatedOn,
                CreatedByUser = post.CreatedByUser.FromModelToMinimalUserDto(),
                Transfer = post.Transfer?.FromModelToMinimalTransferDto(),
                TransferRumour = post.TransferRumour?.FromModelToMinimalTransferRumourDto()
            };
        }
    }
}
