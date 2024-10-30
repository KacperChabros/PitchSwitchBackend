using PitchSwitchBackend.Dtos.Account.Requests;
using PitchSwitchBackend.Dtos.Account.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Mappers
{
    public static class AppUserMapper
    {
        public static NewUserDto FromModelToNewUserDto(this AppUser model, string accessToken, string refreshToken)
        {
            return new NewUserDto
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                FavouriteClub = model.FavouriteClub?.FromModelToMinimalClubDto(),
                Bio = model.Bio,
                ProfilePictureUrl = model.ProfilePictureUrl,
                Tokens = new TokensDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }

        public static AppUser FromRegisterDtoToModel(this RegisterDto registerDto)
        {
            return new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                FavouriteClubId = registerDto.FavouriteClubId,
                ProfilePictureUrl = registerDto.ProfilePictureUrl,
                Bio = registerDto.Bio
            };
        }

        public static UpdateUserDataResultDto FromModelToUpdateUserDataDto(this AppUser appUser)
        {
            return new UpdateUserDataResultDto
            {
                UserName = appUser.UserName,
                Email = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                FavouriteClub = appUser.FavouriteClub?.FromModelToMinimalClubDto(),
                ProfilePictureUrl = appUser.ProfilePictureUrl,
                Bio = appUser.Bio,
            };
        }

        public static UserDto FromModelToUserDto(this AppUser appUser)
        {
            return new UserDto
            {
                UserName = appUser.UserName,
                Email = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                FavouriteClub = appUser.FavouriteClub?.FromModelToMinimalClubDto(),
                ProfilePictureUrl = appUser.ProfilePictureUrl,
                Bio = appUser.Bio,
            };
        }

        public static MinimalUserDto FromModelToMinimalUserDto(this AppUser appUser)
        {
            return new MinimalUserDto
            {
                UserName = appUser.UserName,
                ProfilePictureUrl = appUser.ProfilePictureUrl
            };
        }
    }
}
