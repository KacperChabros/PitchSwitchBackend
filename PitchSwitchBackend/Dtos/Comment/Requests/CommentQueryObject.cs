using PitchSwitchBackend.Helpers;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Comment.Requests
{
    public class CommentQueryObject
    {
        [StringLength(300, MinimumLength = 3)]
        public string? Content { get; set; } = null;
        public DateTime? CreatedOn { get; set; } = null;
        [StringLength(2, MinimumLength = 2)]
        public string CreatedOnComparison { get; set; } = NumberComparisonTypes.Equal;
        public bool? IsEdited { get; set; } = null;
        public string? CreatedByUserId { get; set; } = null;
        public int? PostId { get; set; } = null;
        [StringLength(50)]
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
        [Range(1, 1000)]
        public int PageNumber { get; set; } = 1;
        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
