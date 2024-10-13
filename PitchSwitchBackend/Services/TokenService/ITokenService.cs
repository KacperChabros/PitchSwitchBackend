using PitchSwitchBackend.Models;

namespace PitchSwitchBackend.Services.TokenService
{
    public interface ITokenService
    {
        Task<string> CreateAccessToken(AppUser appUser);
    }
}
