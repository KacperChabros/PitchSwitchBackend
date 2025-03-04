﻿using PitchSwitchBackend.Dtos.Account.Requests;
using PitchSwitchBackend.Dtos.Account.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Mappers
{
    public static class AppUserMapper
    {
        public static NewUserDto FromModelToNewUserDto(this AppUser appUser, string accessToken, string refreshToken)
        {
            return new NewUserDto
            {
                UserId = appUser.Id,
                UserName = appUser.UserName,
                Email = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                FavouriteClub = appUser.FavouriteClub?.FromModelToMinimalClubDto(),
                Bio = appUser.Bio,
                ProfilePictureUrl = appUser.ProfilePictureUrl,
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
                Bio = registerDto.Bio
            };
        }

        public static UpdateUserDataResultDto FromModelToUpdateUserDataDto(this AppUser appUser)
        {
            return new UpdateUserDataResultDto
            {
                UserId = appUser.Id,
                UserName = appUser.UserName,
                Email = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                RegistrationDate = appUser.RegistrationDate,
                FavouriteClub = appUser.FavouriteClub?.FromModelToMinimalClubDto(),
                ProfilePictureUrl = appUser.ProfilePictureUrl,
                Bio = appUser.Bio,
                Posts = appUser.Posts.Select(p => p.FromModelToListElementPostDto()),
                Applications = appUser.Applications.Select(a => a.FromModelToJournalistStatusApplicationDto())
            };
        }

        public static GetUserDto FromModelToGetUserDto(this AppUser appUser)
        {
            return new GetUserDto
            {
                UserName = appUser.UserName,
                Email = appUser.Email,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                FavouriteClub = appUser.FavouriteClub?.FromModelToMinimalClubDto(),
                ProfilePictureUrl = appUser.ProfilePictureUrl,
                Bio = appUser.Bio,
                RegistrationDate = appUser.RegistrationDate,
                Posts = appUser.Posts.Select(p => p.FromModelToListElementPostDto()),
                Applications = appUser.Applications.Select(a => a.FromModelToJournalistStatusApplicationDto())
            };
        }

        public static UserDto FromModelToUserDto(this AppUser appUser)
        {
            return new UserDto
            {
                UserId = appUser.Id,
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
                UserId = appUser.Id,
                UserName = appUser.UserName,
                ProfilePictureUrl = appUser.ProfilePictureUrl
            };
        }
    }
}
