using PitchSwitchBackend.Enums;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Transfer.Requests
{
    public class UpdateTransferDto
    {
        public TransferType? TransferType { get; set; } = null;
        public DateTime? TransferDate { get; set; } = null;
        [Range(0, 1000000000)]
        public decimal? TransferFee { get; set; } = null;
        public int? PlayerId { get; set; } = null;
        public int? SellingClubId { get; set; } = null;
        public int? BuyingClubId { get; set; } = null;
        public bool IsSellingClubDeleted { get; set; } = false;
        public bool IsBuyingClubDeleted { get; set; } = false;
    }
}
