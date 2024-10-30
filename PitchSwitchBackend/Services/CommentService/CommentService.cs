using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos.Comment.Requests;
using PitchSwitchBackend.Dtos.Comment.Responses;
using PitchSwitchBackend.Helpers;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.PostService;

namespace PitchSwitchBackend.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly ApplicationDBContext _context;
        private readonly IPostService _postService;
        public CommentService(ApplicationDBContext context,
            IPostService postService)
        {
            _context = context;
            _postService = postService;
        }

        public async Task<NewCommentDto?> AddComment(AddCommentDto addCommentDto, string userId)
        {
            if (!await ValidatePostExists(addCommentDto.PostId))
            {
                throw new ArgumentException("Given post does not exist");
            }

            var comment = addCommentDto.FromAddCommentDtoToModel();
            comment.CreatedByUserId = userId;
            comment.IsEdited = false;
            comment.CreatedOn = DateTime.UtcNow;
            var result = await _context.Comments.AddAsync(comment);
            await _context.SaveChangesAsync();
            var addedComment = await GetCommentByIdWithAllData(result.Entity.CommentId);

            return addedComment?.FromModelToNewCommentDto();
        }

        public async Task<List<CommentDto>> GetAllComments(CommentQueryObject commentQuery)
        {
            var comments = _context.Comments.AsQueryable();

            comments = FilterComments(comments, commentQuery);

            comments = SortComments(comments, commentQuery);

            var skipNumber = (commentQuery.PageNumber - 1) * commentQuery.PageSize;

            comments = comments.Include(c => c.CreatedByUser).Include(c => c.Post);

            var filteredComments = await comments.Skip(skipNumber).Take(commentQuery.PageSize).ToListAsync();

            return filteredComments.Select(c => c.FromModelToCommentDto()).ToList();
        }

        public async Task<Comment?> GetCommentById(int commentId)
        {
            return await _context.Comments.FirstOrDefaultAsync(c => c.CommentId == commentId);
        }

        public async Task<Comment?> GetCommentByIdWithAllData(int commentId)
        {
            return await _context.Comments.Where(c => c.CommentId == commentId)
                .Include(c => c.CreatedByUser)
                .Include(c => c.Post).ThenInclude(p => p.CreatedByUser)
                .FirstOrDefaultAsync();
        }

        public async Task<CommentDto?> UpdateComment(Comment comment, UpdateCommentDto updateCommentDto)
        {
            if (!string.IsNullOrWhiteSpace(updateCommentDto.Content))
                comment.Content = updateCommentDto.Content;
            comment.IsEdited = true;

            await _context.SaveChangesAsync();

            var updatedPost = await GetCommentByIdWithAllData(comment.CommentId);

            return updatedPost?.FromModelToCommentDto();
        }

        public async Task DeleteComment(Comment comment)
        {
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> ValidatePostExists(int postId)
        {
            return await _postService.PostExists(postId);
        }

        private IQueryable<Comment> FilterComments(IQueryable<Comment> comments, CommentQueryObject commentQuery)
        {
            if (!string.IsNullOrWhiteSpace(commentQuery.Content))
                comments = comments.Where(c => c.Content.ToLower().Contains(commentQuery.Content.ToLower()));

            if (!string.IsNullOrWhiteSpace(commentQuery.CreatedByUserId))
                comments = comments.Where(c => c.CreatedByUserId.ToLower().Contains(commentQuery.CreatedByUserId.ToLower()));


            if (commentQuery.CreatedOn != null)
            {
                comments = commentQuery.CreatedOnComparison switch
                {
                    NumberComparisonTypes.Equal => comments.Where(c => c.CreatedOn.Date == commentQuery.CreatedOn.Value.Date),
                    NumberComparisonTypes.Less => comments.Where(c => c.CreatedOn.Date < commentQuery.CreatedOn.Value.Date),
                    NumberComparisonTypes.LessEqual => comments.Where(c => c.CreatedOn.Date <= commentQuery.CreatedOn.Value.Date),
                    NumberComparisonTypes.Greater => comments.Where(c => c.CreatedOn.Date > commentQuery.CreatedOn.Value.Date),
                    NumberComparisonTypes.GreaterEqual => comments.Where(c => c.CreatedOn.Date >= commentQuery.CreatedOn.Value.Date),
                    _ => comments
                };
            }

            if (commentQuery.IsEdited.HasValue)
                comments = comments.Where(c => c.IsEdited == commentQuery.IsEdited.Value);

            if (commentQuery.PostId.HasValue)
                comments = comments.Where(c => c.PostId == commentQuery.PostId.Value);

            return comments;
        }

        private IQueryable<Comment> SortComments(IQueryable<Comment> comments, CommentQueryObject commentQuery)
        {
            if (!string.IsNullOrWhiteSpace(commentQuery.SortBy))
            {
                if (commentQuery.SortBy.Equals(nameof(CommentQueryObject.Content), StringComparison.OrdinalIgnoreCase))
                    comments = commentQuery.IsDescending ? comments.OrderByDescending(c => c.Content) : comments.OrderBy(c => c.Content);
                else if (commentQuery.SortBy.Equals(nameof(CommentQueryObject.CreatedOn), StringComparison.OrdinalIgnoreCase))
                    comments = commentQuery.IsDescending ? comments.OrderByDescending(c => c.CreatedOn) : comments.OrderBy(c => c.CreatedOn);
            }

            return comments;
        }
    }
}
