using PitchSwitchBackend.Dtos.Club.Responses;
using PitchSwitchBackend.Dtos.Player.Responses;
using PitchSwitchBackend.Enums;

namespace PitchSwitchBackend.Dtos.TransferRumour.Responses
{
    public class TransferRumourDto
    {
        public int TransferRumourId { get; set; }
        public TransferType TransferType { get; set; }
        public decimal RumouredFee { get; set; }
        public int ConfidenceLevel { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsArchived { get; set; }
        public PlayerDto Player { get; set; }
        public ClubDto? SellingClub { get; set; }
        public ClubDto? BuyingClub { get; set; }
    }
}
