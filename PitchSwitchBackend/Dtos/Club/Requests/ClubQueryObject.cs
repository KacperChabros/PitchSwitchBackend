using PitchSwitchBackend.Helpers;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Club.Requests
{
    public class ClubQueryObject
    {
        [StringLength(255)]
        public string? Name { get; set; } = null;
        [StringLength(5)]
        public string? ShortName { get; set; } = null;
        [StringLength(150)]
        public string? League { get; set; } = null;
        [StringLength(150)]
        public string? Country { get; set; } = null;
        [StringLength(150)]
        public string? City { get; set; } = null;
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Foundation Year must be a four-digit number.")]
        public int? FoundationYear { get; set; } = null;
        public string FoundationYearComparison { get; set; } = NumberComparisonTypes.Equal;
        [StringLength(255)]
        public string? Stadium { get; set; } = null;
        [StringLength(50)]
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
        [Range(0, 1000)]
        public int PageNumber { get; set; } = 1;
        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
