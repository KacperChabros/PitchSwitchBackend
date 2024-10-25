using PitchSwitchBackend.Enums;
using PitchSwitchBackend.Helpers;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Player.Requests
{
    public class PlayerQueryObject
    {
        [StringLength(100, MinimumLength = 2)]
        public string? FirstName { get; set; } = null;
        [StringLength(100, MinimumLength = 2)]
        public string? LastName { get; set; } = null;
        public DateTime? DateOfBirth { get; set; } = null;
        public string DateOfBirthComparison { get; set; } = NumberComparisonTypes.Equal;
        [StringLength(100, MinimumLength = 2)]
        public string? Nationality { get; set; } = null;
        [StringLength(100, MinimumLength = 2)]
        public string? Position { get; set; } = null;
        [Range(100, 230)]
        public int? Height { get; set; } = null; // centimeters
        public string HeightComparison { get; set; } = NumberComparisonTypes.Equal;
        [Range(30, 200)]
        public int? Weight { get; set; } = null; // kilograms
        public string WeightComparison { get; set; } = NumberComparisonTypes.Equal;
        public Foot? PreferredFoot { get; set; } = null;
        [Range(0, 1000000000)]
        public decimal? MarketValue { get; set; } = null;
        public string MarketValueComparison { get; set; } = NumberComparisonTypes.Equal;
        public int? ClubId { get; set; } = null;
        public bool? IsUnemployed { get; set; }
        [StringLength(50)]
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
        [Range(0, 1000)]
        public int PageNumber { get; set; } = 1;
        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
