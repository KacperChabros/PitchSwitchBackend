using PitchSwitchBackend.Dtos.Player.Requests;
using PitchSwitchBackend.Dtos.Player.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Mappers
{
    public static class PlayerMapper
    {
        public static Player FromAddNewPlayerDtoToModel(this AddPlayerDto addPlayerDto)
        {
            return new Player
            {
                FirstName = addPlayerDto.FirstName,
                LastName = addPlayerDto.LastName,
                DateOfBirth = addPlayerDto.DateOfBirth,
                Nationality = addPlayerDto.Nationality,
                Position = addPlayerDto.Position,
                Height = addPlayerDto.Height,
                Weight = addPlayerDto.Weight,
                PreferredFoot = addPlayerDto.PreferredFoot,
                MarketValue = addPlayerDto.MarketValue,
                PhotoUrl = addPlayerDto.PhotoUrl,
                ClubId = addPlayerDto.ClubId
            };
        }

        public static NewPlayerDto FromModelToNewPlayerDto(this Player player)
        {
            return new NewPlayerDto
            {
                PlayerId = player.PlayerId,
                FirstName = player.FirstName,
                LastName = player.LastName,
                DateOfBirth = player.DateOfBirth,
                Nationality = player.Nationality,
                Position = player.Position,
                Height = player.Height,
                Weight = player.Weight,
                PreferredFoot = player.PreferredFoot,
                MarketValue = player.MarketValue,
                PhotoUrl = player.PhotoUrl,
                Club = player.Club?.FromModelToMinimalClubDto()
            };
        }

        public static PlayerDto FromModelToPlayerDto(this Player player)
        {
            return new PlayerDto
            {
                PlayerId = player.PlayerId,
                FirstName = player.FirstName,
                LastName = player.LastName,
                DateOfBirth = player.DateOfBirth,
                Nationality = player.Nationality,
                Position = player.Position,
                Height = player.Height,
                Weight = player.Weight,
                PreferredFoot = player.PreferredFoot,
                MarketValue = player.MarketValue,
                PhotoUrl = player.PhotoUrl,
                Club = player.Club?.FromModelToMinimalClubDto()
            };
        }

        public static MinimalPlayerDto FromModelToMinimalPlayerDto(this Player player)
        {
            return new MinimalPlayerDto
            {
                PlayerId = player.PlayerId,
                FirstName = player.FirstName,
                LastName = player.LastName,
                DateOfBirth = player.DateOfBirth,
                Nationality = player.Nationality,
                Position = player.Position,
                MarketValue = player.MarketValue,
                PhotoUrl = player.PhotoUrl,
                Club = player.Club?.FromModelToMinimalClubDto()
            };
        }
    }
}
