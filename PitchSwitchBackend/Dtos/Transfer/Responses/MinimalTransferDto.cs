using PitchSwitchBackend.Dtos.Club.Responses;
using PitchSwitchBackend.Dtos.Player.Responses;
using PitchSwitchBackend.Enums;

namespace PitchSwitchBackend.Dtos.Transfer.Responses
{
    public class MinimalTransferDto
    {
        public int TransferId { get; set; }
        public TransferType TransferType { get; set; }
        public DateTime TransferDate { get; set; }
        public decimal TransferFee { get; set; }
        public MinimalPlayerDto Player { get; set; }
        public MinimalClubDto? SellingClub { get; set; }
        public MinimalClubDto? BuyingClub { get; set; }
    }
}
