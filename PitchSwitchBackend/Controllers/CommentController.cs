using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PitchSwitchBackend.Dtos.Comment.Requests;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Services.CommentService;
using System.Security.Claims;

namespace PitchSwitchBackend.Controllers
{
    [Route("api/comments")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("addcomment")]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] AddCommentDto addCommentDto)
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

            var newComment = await _commentService.AddComment(addCommentDto, userId);

            if (newComment == null)
            {
                return BadRequest(ModelState);
            }

            return CreatedAtAction(nameof(GetComment), new { commentId = newComment.CommentId }, newComment);
        }

        [HttpGet("getallcomments")]
        [Authorize]
        public async Task<IActionResult> GetAllComments([FromQuery] CommentQueryObject commentQuery)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var posts = await _commentService.GetAllComments(commentQuery);

            if (posts == null)
            {
                return NotFound("There are no comments matching the criteria");
            }

            return Ok(posts);
        }

        [HttpGet("getcomment/{commentId:int}")]
        [Authorize]
        public async Task<IActionResult> GetComment([FromRoute] int commentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var comment = await _commentService.GetCommentByIdWithAllData(commentId);

            if (comment == null)
            {
                return NotFound("There is no such comment");
            }

            return Ok(comment.FromModelToCommentDto());
        }

        [HttpPut("updatecomment/{commentId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment([FromRoute] int commentId, [FromBody] UpdateCommentDto updateCommentDto)
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

            var comment = await _commentService.GetCommentById(commentId);

            if (comment == null)
            {
                return NotFound("There is no such comment");
            }

            if (!comment.CreatedByUserId.Equals(userId) && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var updatedComment = await _commentService.UpdateComment(comment, updateCommentDto);

            return Ok(updatedComment);
        }

        [HttpDelete("deletecomment/{commentId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment([FromRoute] int commentId)
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

            var comment = await _commentService.GetCommentByIdWithAllData(commentId);

            if (comment == null)
            {
                return NotFound("There is no such comment");
            }

            if (!comment.CreatedByUserId.Equals(userId) && !User.IsInRole("Admin") && 
                !(User.IsInRole("Journalist") && comment.Post.CreatedByUserId.Equals(userId)))
            {
                return Forbid();
            }

            await _commentService.DeleteComment(comment);

            return NoContent();
        }
    }
}
