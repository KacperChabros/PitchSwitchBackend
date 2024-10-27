using PitchSwitchBackend.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Transfer.Requests
{
    public class AddTransferDto
    {
        [Required]
        public TransferType TransferType { get; set; }
        public DateTime? TransferDate { get; set; } = DateTime.UtcNow;
        [Required]
        [Range(0, 1000000000)]
        public decimal TransferFee { get; set; }
        [Required]
        public int PlayerId { get; set; }
        public int? SellingClubId { get; set; }
        public int? BuyingClubId { get; set; }
    }
}
