using PitchSwitchBackend.Dtos.TransferRumour.Requests;
using PitchSwitchBackend.Dtos.TransferRumour.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Mappers
{
    public static class TransferRumourMapper
    {
        public static TransferRumour FromAddTransferRumourDtoToModel(this AddTransferRumourDto addTransferRumourDto)
        {
            return new TransferRumour
            {
                TransferType = addTransferRumourDto.TransferType,
                RumouredFee = addTransferRumourDto.RumouredFee,
                ConfidenceLevel = addTransferRumourDto.ConfidenceLevel,
                PlayerId = addTransferRumourDto.PlayerId,
                BuyingClubId = addTransferRumourDto.BuyingClubId
            };
        }

        public static NewTransferRumourDto FromModelToNewTransferRumourDto(this TransferRumour transferRumour)
        {
            return new NewTransferRumourDto
            {
                TransferRumourId = transferRumour.TransferRumourId,
                TransferType = transferRumour.TransferType,
                RumouredFee = transferRumour.RumouredFee,
                ConfidenceLevel = transferRumour.ConfidenceLevel,
                Player = transferRumour.Player.FromModelToPlayerDto(),
                SellingClub = transferRumour.SellingClub?.FromModelToClubDto(),
                BuyingClub = transferRumour.BuyingClub?.FromModelToClubDto(),
                IsConfirmed = transferRumour.IsConfirmed,
                IsArchived = transferRumour.IsArchived
            };
        }

        public static TransferRumourDto FromModelToTransferRumourDto(this TransferRumour transferRumour)
        {
            return new TransferRumourDto
            {
                TransferRumourId = transferRumour.TransferRumourId,
                TransferType = transferRumour.TransferType,
                RumouredFee = transferRumour.RumouredFee,
                ConfidenceLevel = transferRumour.ConfidenceLevel,
                Player = transferRumour.Player.FromModelToPlayerDto(),
                SellingClub = transferRumour.SellingClub?.FromModelToClubDto(),
                BuyingClub = transferRumour.BuyingClub?.FromModelToClubDto(),
                IsConfirmed = transferRumour.IsConfirmed,
                IsArchived = transferRumour.IsArchived
            };
        }
    }
}
