using PitchSwitchBackend.Dtos.Club.Requests;
using PitchSwitchBackend.Dtos.Club.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Mappers
{
    public static class ClubMapper
    {
        public static Club FromAddClubDtoToModel(this AddClubDto addClubDto)
        {
            return new Club
            {
                Name = addClubDto.Name,
                ShortName = addClubDto.ShortName,
                League = addClubDto.League,
                Country = addClubDto.Country,
                City = addClubDto.City,
                FoundationYear = addClubDto.FoundationYear,
                Stadium = addClubDto.Stadium,
                LogoUrl = addClubDto.LogoUrl
            };
        }

        public static NewClubDto FromModelToNewClubDto(this Club club)
        {
            return new NewClubDto
            {
                ClubId = club.ClubId,
                Name = club.Name,
                ShortName = club.ShortName,
                League = club.League,
                Country = club.Country,
                City = club.City,
                FoundationYear = club.FoundationYear,
                Stadium = club.Stadium,
                LogoUrl = club.LogoUrl,
                IsArchived = club.IsArchived
            };
        }

        public static ClubDto FromModelToClubDto(this Club club)
        {
            return new ClubDto
            {
                ClubId = club.ClubId,
                Name = club.Name,
                ShortName = club.ShortName,
                League = club.League,
                Country = club.Country,
                City = club.City,
                FoundationYear = club.FoundationYear,
                Stadium = club.Stadium,
                LogoUrl = club.LogoUrl,
                IsArchived = club.IsArchived
            };
        }

        public static MinimalClubDto FromModelToMinimalClubDto(this Club club)
        {
            return new MinimalClubDto
            {
                ClubId = club.ClubId,
                Name = club.Name,
                ShortName = club.ShortName,
                LogoUrl = club.LogoUrl,
            };
        }
    }
}
