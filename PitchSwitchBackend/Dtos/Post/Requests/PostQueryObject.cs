using PitchSwitchBackend.Helpers;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Post.Requests
{
    public class PostQueryObject
    {
        [StringLength(100)]
        public string? Title { get; set; } = null;
        [StringLength(500)]
        public string? Content { get; set; } = null;
        public DateTime? CreatedOn { get; set; } = null;
        [StringLength(2, MinimumLength = 2)]
        public string CreatedOnComparison { get; set; } = NumberComparisonTypes.Equal;
        public int? TransferId { get; set; } = null;
        public bool FilterForEmptyTransferIfEmpty { get; set; } = false;
        public int? TransferRumourId { get; set; } = null;
        public bool FilterForEmptyTransferRumourIfEmpty { get; set; } = false;
        [StringLength(50)]
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
        [Range(1, 1000)]
        public int PageNumber { get; set; } = 1;
        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
