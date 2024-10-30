using PitchSwitchBackend.Dtos.Account.Responses;
using PitchSwitchBackend.Dtos.Club.Responses;
using PitchSwitchBackend.Dtos.Player.Responses;
using PitchSwitchBackend.Enums;

namespace PitchSwitchBackend.Dtos.TransferRumour.Responses
{
    public class NewTransferRumourDto
    {
        public int TransferRumourId { get; set; }
        public TransferType TransferType { get; set; }
        public decimal RumouredFee { get; set; }
        public int ConfidenceLevel { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsArchived { get; set; }
        public MinimalUserDto CreatedByUser { get; set; }
        public MinimalPlayerDto Player { get; set; }
        public MinimalClubDto? SellingClub { get; set; }
        public MinimalClubDto? BuyingClub { get; set; }
    }
}
