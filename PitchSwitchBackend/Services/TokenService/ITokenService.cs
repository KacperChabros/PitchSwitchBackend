using PitchSwitchBackend.Dtos.Account.Responses;
using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.TokenService
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(AppUser appUser);
        Task<string> CreateRefreshToken(AppUser appUser);
        Task<TokensDto> RefreshAccessToken(AppUser appUser, string refreshToken);
    }
}
