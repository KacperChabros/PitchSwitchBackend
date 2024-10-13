using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PitchSwitchBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PitchSwitchBackend.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;
        public TokenService(IConfiguration config,
            UserManager<AppUser> userManager)
        {
            _config = config;
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
        }

        public async Task<string> CreateAccessToken(AppUser appUser)
        {
            var roles = await _userManager.GetRolesAsync(appUser);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, appUser.Email),
                new Claim(JwtRegisteredClaimNames.GivenName, appUser.UserName),
                new Claim(JwtRegisteredClaimNames.Name, appUser.FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, appUser.LastName),
            };

            if (appUser.FavouriteClubId != null)
            {
                claims.Add(new Claim("FavouriteClubId", appUser.FavouriteClubId.ToString()));
            }

            if (!string.IsNullOrWhiteSpace(appUser.ProfilePictureUrl))
            {
                claims.Add(new Claim("ProfilePictureUrl", appUser.ProfilePictureUrl));
            }

            if (!string.IsNullOrWhiteSpace(appUser.Bio))
            {
                claims.Add(new Claim("Bio", appUser.Bio));
            }

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["JWT:AccessTokenExpirationInMinutes"])),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
