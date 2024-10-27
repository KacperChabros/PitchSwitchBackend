using PitchSwitchBackend.Dtos.Club.Responses;
using PitchSwitchBackend.Dtos.Player.Responses;
using PitchSwitchBackend.Enums;

namespace PitchSwitchBackend.Dtos.Transfer.Responses
{
    public class TransferDto
    {
        public int TransferId { get; set; }
        public TransferType TransferType { get; set; }
        public DateTime TransferDate { get; set; }
        public decimal TransferFee { get; set; }
        public PlayerDto Player { get; set; }
        public ClubDto? SellingClub { get; set; }
        public ClubDto? BuyingClub { get; set; }
    }
}
