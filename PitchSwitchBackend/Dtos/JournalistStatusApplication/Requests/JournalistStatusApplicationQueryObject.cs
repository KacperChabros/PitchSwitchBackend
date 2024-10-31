using PitchSwitchBackend.Helpers;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.JournalistStatusApplication.Requests
{
    public class JournalistStatusApplicationQueryObject
    {
        [StringLength(500, MinimumLength = 2)]
        public string? Motivation { get; set; } = null;
        public DateTime? CreatedOn { get; set; } = null;
        [StringLength(2, MinimumLength = 2)]
        public string CreatedOnComparison { get; set; } = NumberComparisonTypes.Equal;
        public bool? IsAccepted { get; set; } = null;
        public bool? IsReviewed { get; set; } = null;
        [StringLength(500, MinimumLength = 2)]
        public string? RejectionReason { get; set; } = null;
        public string? SubmittedByUserId { get; set; } = null;
        [StringLength(50)]
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
        [Range(1, 1000)]
        public int PageNumber { get; set; } = 1;
        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
