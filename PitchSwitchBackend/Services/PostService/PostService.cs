using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos;
using PitchSwitchBackend.Dtos.Club.Requests;
using PitchSwitchBackend.Dtos.Post.Requests;
using PitchSwitchBackend.Dtos.Post.Responses;
using PitchSwitchBackend.Helpers;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.ImageService;
using PitchSwitchBackend.Services.TransferRumourService;
using PitchSwitchBackend.Services.TransferService;
using System.Security.Cryptography.Xml;

namespace PitchSwitchBackend.Services.PostService
{
    public class PostService : IPostService
    {
        private readonly ApplicationDBContext _context;
        private readonly ITransferService _transferService;
        private readonly ITransferRumourService _transferRumourService;
        private readonly IImageService _imageService;
        public PostService(ApplicationDBContext context,
            ITransferService transferService,
            ITransferRumourService transferRumourService,
            IImageService imageService)
        {
            _context = context;
            _transferService = transferService;
            _transferRumourService = transferRumourService;
            _imageService = imageService;
        }

        public async Task<bool> PostExists(int postId)
        {
            return await _context.Posts.AnyAsync(p => p.PostId == postId);
        }

        public async Task<NewPostDto?> AddPost(AddPostDto addPostDto, string userId)
        {
            await ValidateAddPostData(addPostDto);
            var post = addPostDto.FromAddPostDtoToModel();

            if (addPostDto.Image != null)
            {
                var imagePath = await _imageService.UploadFileAsync(addPostDto.Image, UploadFolders.PostsDir);
                post.ImageUrl = imagePath;
            }

            post.CreatedByUserId = userId;
            post.CreatedOn = DateTime.UtcNow;
            post.IsEdited = false;

            var result = await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            var newPost = result.Entity;
            if (newPost.TransferId.HasValue)
                await _transferService.GetTransferWithDataById(newPost.TransferId.Value);
            if (newPost.TransferRumourId.HasValue)
                await _transferRumourService.GetTransferRumourWithDataById(newPost.TransferRumourId.Value);

            await _context.Entry(newPost).Reference(p => p.CreatedByUser).LoadAsync();

            return result.Entity?.FromModelToNewPostDto();
        }

        public async Task<PaginatedListDto<ListElementPostDto>> GetAllPosts(PostQueryObject postQuery)
        {
            var posts = _context.Posts.AsQueryable();

            posts = FilterPosts(posts, postQuery);

            posts = SortPosts(posts, postQuery);

            var totalCount = await posts.CountAsync();

            var skipNumber = (postQuery.PageNumber - 1) * postQuery.PageSize;

            posts = IncludeRelatedDataForList(posts);

            var filteredPosts = await posts.Skip(skipNumber).Take(postQuery.PageSize).ToListAsync();

            var paginatedPosts = filteredPosts.Select(p => p.FromModelToListElementPostDto()).ToList();

            return new PaginatedListDto<ListElementPostDto>
            {
                Items = paginatedPosts,
                TotalCount = totalCount,
            };
        }

        public async Task<Post?> GetPostById(int postId)
        {
            return await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId);
        }

        public async Task<Post?> GetPostWithDataById(int postId)
        {
            return await _context.Posts.Where(p => p.PostId == postId)
                .Include(p => p.CreatedByUser)
                .Include(p => p.Transfer).ThenInclude(t => t.Player)
                .Include(p => p.Transfer).ThenInclude(t => t.SellingClub)
                .Include(p => p.Transfer).ThenInclude(t => t.BuyingClub)
                .Include(p => p.TransferRumour).ThenInclude(tr => tr.Player)
                .Include(p => p.TransferRumour).ThenInclude(tr => tr.SellingClub)
                .Include(p => p.TransferRumour).ThenInclude(tr => tr.BuyingClub)
                .Include(p => p.TransferRumour).ThenInclude(tr => tr.CreatedByUser)
                .Include(p => p.Comments).ThenInclude(c => c.CreatedByUser)
                .FirstOrDefaultAsync();
        }

        public async Task<PostDto?> UpdatePost(Post post, UpdatePostDto updatePostDto)
        {
            await ValidateUpdatePostData(post, updatePostDto);

            if (updatePostDto.Image != null)
            {
                var oldImageUrl = post.ImageUrl;
                var newImageUrl = await _imageService.UploadFileAsync(updatePostDto.Image, UploadFolders.PostsDir);
                post.ImageUrl = newImageUrl;
                if (!string.IsNullOrEmpty(oldImageUrl))
                    _imageService.DeleteFile(oldImageUrl);
            }
            else if (updatePostDto.IsImageDeleted)
            {
                if (!string.IsNullOrEmpty(post.ImageUrl))
                    _imageService.DeleteFile(post.ImageUrl);
                post.ImageUrl = null;
            }


            if (!string.IsNullOrWhiteSpace(updatePostDto.Title))
                post.Title = updatePostDto.Title;

            if (!string.IsNullOrWhiteSpace(updatePostDto.Content))
                post.Content = updatePostDto.Content;

            if (updatePostDto.IsTransferDeleted)
                post.TransferId = null;
            else if (updatePostDto.TransferId.HasValue)
                post.TransferId = updatePostDto.TransferId.Value;

            if (updatePostDto.IsTransferRumourDeleted)
                post.TransferRumourId = null;
            else if (updatePostDto.TransferRumourId.HasValue)
                post.TransferRumourId = updatePostDto.TransferRumourId.Value;

            post.IsEdited = true;

            await _context.SaveChangesAsync();

            var updatedPost = await GetPostWithDataById(post.PostId);

            return updatedPost?.FromModelToPostDto();
        }

        public async Task DeletePost(Post post)
        {
            var comments = post.Comments;
            _context.Comments.RemoveRange(comments);
            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
        }

        private async Task ValidateAddPostData(AddPostDto addPostDto)
        {
            if (addPostDto.TransferId.HasValue && addPostDto.TransferRumourId.HasValue)
            {
                throw new ArgumentException("The post cannot reference both transfer and transfer rumour simultaneously");
            }

            if (addPostDto.TransferId.HasValue && !await _transferService.TransferExists(addPostDto.TransferId.Value))
            {
                throw new ArgumentException("The transfer does not exist");
            }

            if (addPostDto.TransferRumourId.HasValue && !await _transferRumourService.TransferRumourExists(addPostDto.TransferRumourId.Value))
            {
                throw new ArgumentException("The transfer rumour does not exist");
            }
        }

        private async Task ValidateUpdatePostData(Post post, UpdatePostDto updatePostDto)
        {
            int? newTransferId = updatePostDto.IsTransferDeleted ? null : updatePostDto.TransferId ?? post.TransferId;
            int? newTransferRumourId = updatePostDto.IsTransferRumourDeleted ? null : updatePostDto.TransferRumourId ?? post.TransferRumourId;

            if (newTransferId.HasValue && newTransferRumourId.HasValue)
            {
                throw new ArgumentException("The post cannot reference both transfer and transfer rumour simultaneously");
            }

            if (newTransferId.HasValue && !await _transferService.TransferExists(newTransferId.Value))
            {
                throw new ArgumentException("The transfer does not exist");
            }

            if (newTransferRumourId.HasValue && !await _transferRumourService.TransferRumourExists(newTransferRumourId.Value))
            {
                throw new ArgumentException("The transfer rumour does not exist");
            }
        }

        private IQueryable<Post> FilterPosts(IQueryable<Post> posts, PostQueryObject postQuery)
        {
            if (!string.IsNullOrWhiteSpace(postQuery.Title))
                posts = posts.Where(p => p.Title.ToLower().Contains(postQuery.Title.ToLower()));

            if (!string.IsNullOrWhiteSpace(postQuery.Content))
                posts = posts.Where(p => p.Content.ToLower().Contains(postQuery.Content.ToLower()));


            if (postQuery.CreatedOn != null)
            {
                posts = postQuery.CreatedOnComparison switch
                {
                    NumberComparisonTypes.Equal => posts.Where(p => p.CreatedOn.Date == postQuery.CreatedOn.Value.Date),
                    NumberComparisonTypes.Less => posts.Where(p => p.CreatedOn.Date < postQuery.CreatedOn.Value.Date),
                    NumberComparisonTypes.LessEqual => posts.Where(p => p.CreatedOn.Date <= postQuery.CreatedOn.Value.Date),
                    NumberComparisonTypes.Greater => posts.Where(p => p.CreatedOn.Date > postQuery.CreatedOn.Value.Date),
                    NumberComparisonTypes.GreaterEqual => posts.Where(p => p.CreatedOn.Date >= postQuery.CreatedOn.Value.Date),
                    _ => posts
                };
            }

            if (postQuery.TransferId.HasValue)
            {
                posts = posts.Where(p => p.TransferId == postQuery.TransferId.Value);
            }
            else if (postQuery.FilterForEmptyTransferIfEmpty)
            {
                posts = posts.Where(p => p.TransferId == null);
            }

            if (postQuery.TransferRumourId.HasValue)
            {
                posts = posts.Where(p => p.TransferRumourId == postQuery.TransferRumourId.Value);
            }
            else if (postQuery.FilterForEmptyTransferRumourIfEmpty)
            {
                posts = posts.Where(p => p.TransferRumourId == null);
            }

            return posts;
        }

        private IQueryable<Post> SortPosts(IQueryable<Post> posts, PostQueryObject postQuery)
        {
            if (!string.IsNullOrWhiteSpace(postQuery.SortBy))
            {
                if (postQuery.SortBy.Equals(nameof(PostQueryObject.Title), StringComparison.OrdinalIgnoreCase))
                    posts = postQuery.IsDescending ? posts.OrderByDescending(p => p.Title) : posts.OrderBy(p => p.Title);
                else if (postQuery.SortBy.Equals(nameof(PostQueryObject.Content), StringComparison.OrdinalIgnoreCase))
                    posts = postQuery.IsDescending ? posts.OrderByDescending(p => p.Content) : posts.OrderBy(p => p.Content);
                else if (postQuery.SortBy.Equals(nameof(PostQueryObject.CreatedOn), StringComparison.OrdinalIgnoreCase))
                    posts = postQuery.IsDescending ? posts.OrderByDescending(p => p.CreatedOn) : posts.OrderBy(p => p.CreatedOn);
            }

            return posts;
        }

        private IQueryable<Post> IncludeRelatedDataForList(IQueryable<Post> posts)
        {
            posts = posts.Include(p => p.CreatedByUser)
                .Include(p => p.Transfer)
                    .ThenInclude(t => t.Player)
                .Include(p => p.Transfer)
                    .ThenInclude(t => t.SellingClub)
                .Include(p => p.Transfer)
                    .ThenInclude(t => t.BuyingClub)
                .Include(p => p.TransferRumour)
                    .ThenInclude(t => t.Player)
                .Include(p => p.TransferRumour)
                    .ThenInclude(t => t.SellingClub)
                .Include(p => p.TransferRumour)
                    .ThenInclude(tr => tr.BuyingClub)
                .Include(p => p.TransferRumour)
                        .ThenInclude(tr => tr.CreatedByUser);
            return posts;
        }
    }
}
