using PitchSwitchBackend.Dtos.Account.Requests;
using PitchSwitchBackend.Dtos.Account.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Mappers
{
    public static class AppUserMapper
    {
        public static NewUserDto ToNewUserDtoFromModel(this AppUser model, string accessToken, string refreshToken)
        {
            return new NewUserDto
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                FavouriteClubId = model.FavouriteClubId,
                Bio = model.Bio,
                ProfilePictureUrl = model.ProfilePictureUrl,
                Tokens = new TokensDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }

        public static AppUser ToAppUserFromRegisterDto(this RegisterDto registerDto)
        {
            return new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                FavouriteClubId = registerDto.FavouriteClub,
                ProfilePictureUrl = registerDto.ProfilePictureUrl,
                Bio = registerDto.Bio
            };
        }
    }
}
