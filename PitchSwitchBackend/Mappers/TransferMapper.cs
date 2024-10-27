using PitchSwitchBackend.Dtos.Transfer.Requests;
using PitchSwitchBackend.Dtos.Transfer.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Mappers
{
    public static class TransferMapper
    {
        public static NewTransferDto FromModelToNewTransferDto(this Transfer transfer)
        {
            return new NewTransferDto
            {
                TransferId = transfer.TransferId,
                TransferDate = transfer.TransferDate,
                TransferType = transfer.TransferType,
                TransferFee = transfer.TransferFee,
                Player = transfer.Player.FromModelToPlayerDto(),
                SellingClub = transfer.SellingClub?.FromModelToClubDto(),
                BuyingClub = transfer.BuyingClub?.FromModelToClubDto()
            };
        }

        public static TransferDto FromModelToTransferDto(this Transfer transfer)
        {
            return new TransferDto
            {
                TransferId = transfer.TransferId,
                TransferDate = transfer.TransferDate,
                TransferType = transfer.TransferType,
                TransferFee = transfer.TransferFee,
                Player = transfer.Player.FromModelToPlayerDto(),
                SellingClub = transfer.SellingClub?.FromModelToClubDto(),
                BuyingClub = transfer.BuyingClub?.FromModelToClubDto()
            };
        }

        public static Transfer FromAddTransferDtoToModel(this AddTransferDto addTransferDto)
        {
            return new Transfer
            {
                TransferType = addTransferDto.TransferType,
                TransferDate = addTransferDto.TransferDate.GetValueOrDefault(),
                TransferFee = addTransferDto.TransferFee,
                PlayerId = addTransferDto.PlayerId,
                SellingClubId = addTransferDto.SellingClubId,
                BuyingClubId = addTransferDto.BuyingClubId
            };
        }
    }
}
