using PitchSwitchBackend.Enums;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.TransferRumour.Requests
{
    public class UpdateTransferRumourDto
    {
        public TransferType? TransferType { get; set; } = null;
        [Range(0, 1000000000)]
        public decimal? RumouredFee { get; set; } = null;
        [Range(1, 100)]
        public int? ConfidenceLevel { get; set; } = null;
        public int? PlayerId { get; set; } = null;
        public int? BuyingClubId { get; set; } = null;
        public bool IsBuyingClubDeleted { get; set; } = false;
    }
}
