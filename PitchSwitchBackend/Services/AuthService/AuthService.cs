using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos.Account.Requests;
using PitchSwitchBackend.Dtos.Account.Responses;
using PitchSwitchBackend.Helpers;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.ClubService;
using PitchSwitchBackend.Services.ImageService;
using PitchSwitchBackend.Services.TokenService;

namespace PitchSwitchBackend.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDBContext _context;
        private readonly ITokenService _tokenService;
        private readonly IClubService _clubService;
        private readonly IImageService _imageService;
        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ApplicationDBContext context,
            ITokenService tokenService,
            IClubService clubService,
            IImageService imageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _tokenService = tokenService;
            _clubService = clubService;
            _imageService = imageService;
        }

        public async Task<IdentityResultDto<NewUserDto>> RegisterUser(RegisterDto registerDto)
        {
            var appUser = registerDto.FromRegisterDtoToModel();

            if (registerDto.ProfilePicture != null)
            {
                var profilePictureUrl = await _imageService.UploadFileAsync(registerDto.ProfilePicture, UploadFolders.UsersDir);
                if(!string.IsNullOrWhiteSpace(profilePictureUrl))
                    appUser.ProfilePictureUrl = profilePictureUrl;
            }
                

            var createdUserResult = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (createdUserResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                if (roleResult.Succeeded)
                {
                    var accessToken = await _tokenService.CreateAccessToken(appUser);
                    var refreshToken = await _tokenService.CreateRefreshToken(appUser);
                    if (appUser.FavouriteClubId != null)
                    {
                        appUser.FavouriteClub = await _clubService.GetClubById((int)appUser.FavouriteClubId);
                    }

                    return IdentityResultDto<NewUserDto>.Succeeded(appUser.FromModelToNewUserDto(accessToken, refreshToken));
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
            var user = await _userManager.Users.Include(u => u.FavouriteClub).FirstOrDefaultAsync(x => x.UserName.Equals(loginDto.Username.ToLower()));
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

            return ResultDto<NewUserDto>.Succeeded(user.FromModelToNewUserDto(accessToken, refreshToken));
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

        public async Task<AppUser?> FindUserByName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }
        public async Task<AppUser?> FindUserByNameWithData(string userName)
        {
            var appUsers = _context.Users.AsQueryable().Where(u => u.UserName == userName);
            var appUser = await appUsers.Include(u => u.FavouriteClub).Include(u => u.Posts).ThenInclude(p => p.Comments).FirstOrDefaultAsync();
            return appUser;
        }


        public async Task<IdentityResultDto<UpdateUserDataResultDto>> UpdateUserData(AppUser appUser, UpdateUserDataDto updateUserDataDto)
        {
            if (!await ValidateClubExists(updateUserDataDto.FavouriteClubId))
            {
                throw new ArgumentException("Given club does not exist");
            }

            if (updateUserDataDto.ProfilePicture != null)
            {
                var oldPictureUrl = appUser.ProfilePictureUrl;
                var newPictureUrl = await _imageService.UploadFileAsync(updateUserDataDto.ProfilePicture, UploadFolders.UsersDir);
                appUser.ProfilePictureUrl = newPictureUrl;
                if (!string.IsNullOrEmpty(oldPictureUrl))
                    _imageService.DeleteFile(oldPictureUrl);
            }
            else if (updateUserDataDto.IsProfilePictureDeleted)
            {
                if (!string.IsNullOrEmpty(appUser.ProfilePictureUrl))
                    _imageService.DeleteFile(appUser.ProfilePictureUrl);
                appUser.ProfilePictureUrl = null;
            }

            if (!string.IsNullOrWhiteSpace(updateUserDataDto.FirstName))
                appUser.FirstName = updateUserDataDto.FirstName;

            if (!string.IsNullOrWhiteSpace(updateUserDataDto.LastName))
                appUser.LastName = updateUserDataDto.LastName;

            if (!string.IsNullOrWhiteSpace(updateUserDataDto.Email))
                appUser.Email = updateUserDataDto.Email;

            if (updateUserDataDto.IsBioDeleted)
                appUser.Bio = null;
            else if (!string.IsNullOrWhiteSpace(updateUserDataDto.Bio))
                appUser.Bio = updateUserDataDto.Bio;

            if (updateUserDataDto.IsFavouriteClubIdDeleted)
            {
                appUser.FavouriteClubId = null;
            }
            else if (updateUserDataDto.FavouriteClubId.HasValue)
            {
                if (!await ValidateClubExists(updateUserDataDto.FavouriteClubId))
                {
                    throw new ArgumentException("Given club does not exist");
                }
                appUser.FavouriteClubId = updateUserDataDto.FavouriteClubId.Value;
            }

            var result = await _userManager.UpdateAsync(appUser);
            if (result.Succeeded)
            {
                if (appUser.FavouriteClubId.HasValue)
                {
                    appUser.FavouriteClub = await _clubService.GetClubById(appUser.FavouriteClubId.GetValueOrDefault());
                }


                return IdentityResultDto<UpdateUserDataResultDto>.Succeeded(appUser.FromModelToUpdateUserDataDto());
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
            foreach (var post in appUser.Posts)
            {
                _context.Comments.RemoveRange(post.Comments);
            }
            _context.Posts.RemoveRange(appUser.Posts);
            await _context.SaveChangesAsync();
            var result = await _userManager.DeleteAsync(appUser);

            if (result.Succeeded)
            {
                return IdentityResultDto<string>.Succeeded("User has been successfully deleted");
            }

            return IdentityResultDto<string>.Failed(result.Errors);
        }

        public async Task AddUserToRole(AppUser appUser, string role)
        {
            string[] validRoles = { "User", "Journalist", "Admin" };
            if (string.IsNullOrWhiteSpace(role) || !validRoles.Contains(role))
                throw new ArgumentException("Invalid Role");
            var roleResult = await _userManager.AddToRoleAsync(appUser, role);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(roleResult.Errors.ToString());
            }
        }

        private async Task<bool> ValidateClubExists(int? clubId)
        {
            return clubId == null || await _clubService.ClubExistsAndNotArchived(clubId.Value);
        }
    }
}
