using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos.Account.Requests;
using PitchSwitchBackend.Dtos.Account.Responses;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.TokenService;

namespace PitchSwitchBackend.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDBContext _context;
        private readonly ITokenService _tokenService;
        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ApplicationDBContext context,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<IdentityResultDto<NewUserDto>> RegisterUser(RegisterDto registerDto)
        {
            var appUser = registerDto.ToAppUserFromRegisterDto();

            var createdUserResult = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (createdUserResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                if (roleResult.Succeeded)
                {
                    var accessToken = await _tokenService.CreateAccessToken(appUser);
                    var refreshToken = await _tokenService.CreateRefreshToken(appUser);

                    return IdentityResultDto<NewUserDto>.Succeeded(appUser.ToNewUserDtoFromModel(accessToken, refreshToken));
                }
                else
                {
                    return IdentityResultDto<NewUserDto>.Failed(roleResult.Errors);
                }
            }
            else
            {
                return IdentityResultDto<NewUserDto>.Failed(createdUserResult.Errors);
            }
        }

        public async Task<ResultDto<NewUserDto>> LoginUser(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName.Equals(loginDto.Username.ToLower()));
            if (user == null)
            {
                return ResultDto<NewUserDto>.Failed("Username and/or password is invalid");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
            {
                return ResultDto<NewUserDto>.Failed("Username and/or password is invalid");
            }

            var accessToken = await _tokenService.CreateAccessToken(user);
            var refreshToken = await _tokenService.CreateRefreshToken(user);

            return ResultDto<NewUserDto>.Succeeded(user.ToNewUserDtoFromModel(accessToken, refreshToken));
        }

        public async Task<ResultDto<TokensDto>> RefreshToken(RefreshTokenRequestDto refreshTokenRequestDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(refreshTokenRequestDto.UserName));

            if (user == null)
            {
                return ResultDto<TokensDto>.Failed("Refreshing the token failed");
            }

            var tokens = await _tokenService.RefreshAccessToken(user, refreshTokenRequestDto.RefreshToken);

            if (tokens == null)
            {
                return ResultDto<TokensDto>.Failed("Refreshing the token failed");
            }

            return ResultDto<TokensDto>.Succeeded(tokens);
        }

        public async Task<AppUser> FindUserByName(string userName)
        {
            var appUser = await _userManager.FindByNameAsync(userName);

            return appUser;
        }

        public async Task<IdentityResultDto<UpdateUserDataResultDto>> UpdateUserData(AppUser appUser, UpdateUserDataDto updateUserDataDto)
        {
            if (!string.IsNullOrWhiteSpace(updateUserDataDto.FirstName))
            {
                appUser.FirstName = updateUserDataDto.FirstName;
            }

            if (!string.IsNullOrWhiteSpace(updateUserDataDto.LastName))
            {
                appUser.LastName = updateUserDataDto.LastName;
            }

            if (!string.IsNullOrWhiteSpace(updateUserDataDto.Email))
            {
                appUser.Email = updateUserDataDto.Email;
            }

            if (updateUserDataDto.IsProfilePictureUrlDeleted)
            {
                appUser.ProfilePictureUrl = null;
            }
            else if (!string.IsNullOrWhiteSpace(updateUserDataDto.ProfilePictureUrl))
            {
                appUser.ProfilePictureUrl = updateUserDataDto.ProfilePictureUrl;
            }

            if (updateUserDataDto.IsBioDeleted)
            {
                appUser.Bio = null;
            }
            else if (!string.IsNullOrWhiteSpace(updateUserDataDto.Bio))
            {
                appUser.Bio = updateUserDataDto.Bio;
            }

            if (updateUserDataDto.IsFavouriteClubIdDeleted)
            {
                appUser.FavouriteClubId = null;
            }
            else if (updateUserDataDto.FavouriteClubId.HasValue)
            {
                appUser.FavouriteClubId = updateUserDataDto.FavouriteClubId.Value;
            }

            var result = await _userManager.UpdateAsync(appUser);
            if (result.Succeeded)
            {
                return IdentityResultDto<UpdateUserDataResultDto>.Succeeded(appUser.ToUpdateUserDataDtoFromModel());
            }

            return IdentityResultDto<UpdateUserDataResultDto>.Failed(result.Errors);
        }

        public async Task<IdentityResultDto<string>> ChangePassword(AppUser appUser, ChangePasswordDto changePasswordDto)
        {
            var result = await _userManager.ChangePasswordAsync(appUser, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (result.Succeeded)
            {
                return IdentityResultDto<string>.Succeeded("The password has been successfully changed");
            }

            return IdentityResultDto<string>.Failed(result.Errors);
        }

        public async Task<IdentityResultDto<string>> DeleteUser(AppUser appUser)
        {
            // delete associated data

            var result = await _userManager.DeleteAsync(appUser);

            if (result.Succeeded)
            {
                return IdentityResultDto<string>.Succeeded("User has been successfully deleted");
            }

            return IdentityResultDto<string>.Failed(result.Errors);
        }
    }
}
