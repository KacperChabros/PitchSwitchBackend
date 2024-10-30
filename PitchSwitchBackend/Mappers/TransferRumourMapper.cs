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
                CreatedByUser = transferRumour.CreatedByUser.FromModelToMinimalUserDto(),
                Player = transferRumour.Player.FromModelToMinimalPlayerDto(),
                SellingClub = transferRumour.SellingClub?.FromModelToMinimalClubDto(),
                BuyingClub = transferRumour.BuyingClub?.FromModelToMinimalClubDto(),
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
                CreatedByUser = transferRumour.CreatedByUser.FromModelToMinimalUserDto(),
                Player = transferRumour.Player.FromModelToMinimalPlayerDto(),
                SellingClub = transferRumour.SellingClub?.FromModelToMinimalClubDto(),
                BuyingClub = transferRumour.BuyingClub?.FromModelToMinimalClubDto(),
                IsConfirmed = transferRumour.IsConfirmed,
                IsArchived = transferRumour.IsArchived
            };
        }

        public static MinimalTransferRumourDto FromModelToMinimalTransferRumourDto(this TransferRumour transferRumour)
        {
            return new MinimalTransferRumourDto
            {
                TransferRumourId = transferRumour.TransferRumourId,
                TransferType = transferRumour.TransferType,
                RumouredFee = transferRumour.RumouredFee,
                ConfidenceLevel = transferRumour.ConfidenceLevel,
                IsConfirmed = transferRumour.IsConfirmed,
                IsArchived = transferRumour.IsArchived,
                CreatedByUser = transferRumour.CreatedByUser.FromModelToMinimalUserDto(),
                Player = transferRumour.Player.FromModelToMinimalPlayerDto(),
                SellingClub = transferRumour.SellingClub?.FromModelToMinimalClubDto(),
                BuyingClub = transferRumour.BuyingClub?.FromModelToMinimalClubDto(),
            };
        }
    }
}
