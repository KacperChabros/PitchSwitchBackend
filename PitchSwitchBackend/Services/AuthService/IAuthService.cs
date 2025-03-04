﻿using PitchSwitchBackend.Dtos.Account.Requests;
using PitchSwitchBackend.Dtos.Account.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.AuthService
{
    public interface IAuthService
    {
        Task<IdentityResultDto<NewUserDto>> RegisterUser(RegisterDto registerDto);
        Task<ResultDto<NewUserDto>> LoginUser(LoginDto loginDto);
        Task<ResultDto<TokensDto>> RefreshToken(RefreshTokenRequestDto refreshTokenRequestDto);
        Task<AppUser?> FindUserByName(string userName);
        Task<AppUser?> FindUserByNameWithData(string userName);
        Task<List<MinimalUserDto>> GetAllMinUsers();
        Task<IdentityResultDto<UpdateUserDataResultDto>> UpdateUserData(AppUser appUser, UpdateUserDataDto updateUseDataDto);
        Task<IdentityResultDto<string>> ChangePassword(AppUser appUser, ChangePasswordDto changePasswordDto);
        Task<IdentityResultDto<string>> DeleteUser(AppUser appUser);
        Task AddUserToRole(AppUser appUser, string role);
    }
}
