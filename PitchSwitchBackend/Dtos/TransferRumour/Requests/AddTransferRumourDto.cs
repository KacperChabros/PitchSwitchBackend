using PitchSwitchBackend.Enums;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.TransferRumour.Requests
{
    public class AddTransferRumourDto
    {
        [Required]
        public TransferType TransferType { get; set; }
        [Required]
        [Range(0, 1000000000)]
        public decimal RumouredFee { get; set; }
        [Required]
        [Range(1, 10)]
        public int ConfidenceLevel { get; set; }
        [Required]
        public int PlayerId { get; set; }
        public int? BuyingClubId { get; set; }
    }
}
