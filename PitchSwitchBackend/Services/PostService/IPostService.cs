using PitchSwitchBackend.Dtos.Post.Requests;
using PitchSwitchBackend.Dtos.Post.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.PostService
{
    public interface IPostService
    {
        Task<bool> PostExists(int postId);
        Task<NewPostDto?> AddPost(AddPostDto addPostDto, string userId);
        Task<List<ListElementPostDto>> GetAllPosts(PostQueryObject postQuery);
        Task<Post?> GetPostById(int postId);
        Task<Post?> GetPostWithDataById(int postId);
        Task<PostDto?> UpdatePost(Post post, UpdatePostDto updatePostDto);
        Task DeletePost(Post post);
    }
}
