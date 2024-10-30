using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PitchSwitchBackend.Dtos.Post.Requests;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Services.PostService;
using System.Security.Claims;

namespace PitchSwitchBackend.Controllers
{
    [Route("api/posts")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost("addpost")]
        [Authorize(Roles = "Admin, Journalist")]
        public async Task<IActionResult> AddPost([FromBody] AddPostDto addPostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var newPost = await _postService.AddPost(addPostDto, userId);

            if (newPost == null)
            {
                return BadRequest(ModelState);
            }

            return CreatedAtAction(nameof(GetPost), new { postId = newPost.PostId }, newPost);
        }

        [HttpGet("getallposts")]
        [Authorize]
        public async Task<IActionResult> GetAllClubs([FromQuery] PostQueryObject postQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var posts = await _postService.GetAllPosts(postQuery);

            if (posts == null)
            {
                return NotFound("There are no posts matching the criteria");
            }

            return Ok(posts);
        }

        [HttpGet("getpost/{postId:int}")]
        [Authorize]
        public async Task<IActionResult> GetPost([FromRoute] int postId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var post = await _postService.GetPostWithDataById(postId);

            if (post == null)
            {
                return NotFound("There is no such post");
            }

            return Ok(post.FromModelToPostDto());
        }

        [HttpPut("updatepost/{postId:int}")]
        [Authorize(Roles = "Admin, Journalist")]
        public async Task<IActionResult> UpdatePost([FromRoute] int postId, [FromBody] UpdatePostDto updatePostDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var post = await _postService.GetPostById(postId);

            if (post == null)
            {
                return NotFound("There is no such post");
            }

            if (!post.CreatedByUserId.Equals(userId) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var updatedPost = await _postService.UpdatePost(post, updatePostDto);

            return Ok(updatedPost);
        }

        [HttpDelete("deletepost/{postId:int}")]
        [Authorize(Roles = "Admin, Journalist")]
        public async Task<IActionResult> DeletePost([FromRoute] int postId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var post = await _postService.GetPostWithDataById(postId);

            if (post == null)
            {
                return NotFound("There is no such post");
            }

            if (!post.CreatedByUserId.Equals(userId) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            await _postService.DeletePost(post);

            return NoContent();
        }
    }
}
