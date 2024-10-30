using PitchSwitchBackend.Enums;
using PitchSwitchBackend.Helpers;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.TransferRumour.Requests
{
    public class TransferRumourQueryObject
    {
        public TransferType? TransferType { get; set; } = null;
        [Range(0, 1000000000)]
        public decimal? RumouredFee { get; set; } = null;
        [StringLength(2, MinimumLength = 2)]
        public string RumouredFeeComparison { get; set; } = NumberComparisonTypes.Equal;
        [Range(1, 10)]
        public int? ConfidenceLevel { get; set; } = null;
        [StringLength(2, MinimumLength = 2)]
        public string ConfidenceLevelComparison { get; set; } = NumberComparisonTypes.Equal;
        public int? PlayerId { get; set; } = null;
        public int? SellingClubId { get; set; } = null;
        public bool FilterForEmptySellingClubIfEmpty { get; set; } = false;
        public int? BuyingClubId { get; set; } = null;
        public bool FilterForEmptyBuyingClubIfEmpty { get; set; } = false;
        [StringLength(50)]
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
        [Range(1, 1000)]
        public int PageNumber { get; set; } = 1;
        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
