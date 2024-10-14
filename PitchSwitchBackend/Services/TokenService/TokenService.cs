using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos.Account;
using PitchSwitchBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace PitchSwitchBackend.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<AppUser> _userManager;
        private readonly ApplicationDBContext _context;
        public TokenService(IConfiguration config,
            UserManager<AppUser> userManager,
            ApplicationDBContext context)
        {
            _config = config;
            _userManager = userManager;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]));
            _context = context;
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

        public async Task<string> CreateRefreshToken(AppUser appUser)
        {
            var refreshToken = GenerateRandomToken();
            var refreshTokenHash = HashToken(refreshToken);

            var newRefreshToken = new RefreshToken
            {
                RefreshTokenHash = refreshTokenHash,
                ExpiryDate = DateTime.UtcNow.AddHours(double.Parse(_config["JWT:RefreshTokenExpirationInHours"])),
                AppUserId = appUser.Id
            };

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        public async Task<TokensDto> RefreshAccessToken(AppUser appUser, string refreshToken)
        {
            var refreshTokenHash = HashToken(refreshToken);

            var refreshTokenRecord = await _context.RefreshTokens.Where(rt => rt.AppUserId.Equals(appUser.Id) && 
                                                            rt.RefreshTokenHash.Equals(refreshTokenHash) &&
                                                            DateTime.UtcNow < rt.ExpiryDate).FirstOrDefaultAsync();

            if (refreshTokenRecord == null)
            {
                throw new UnauthorizedAccessException("Unauthorized Access");
            }

            var newAccessToken = await CreateAccessToken(appUser);
            var newRefreshToken = await RotateRefreshToken(refreshTokenRecord);

            return new TokensDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        private string GenerateRandomToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private string HashToken(string token)
        {
            using (var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(_config["JWT:RefreshSigningKey"])))
            {
                var tokenBytes = Encoding.UTF8.GetBytes(token);
                var hashedBytes = hmac.ComputeHash(tokenBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private async Task<string> RotateRefreshToken(RefreshToken refreshToken)
        {
            var newRefreshToken = GenerateRandomToken();
            var newRefreshTokenHashed = HashToken(newRefreshToken);

            refreshToken.RefreshTokenHash = newRefreshTokenHashed;

            await _context.SaveChangesAsync();
            return newRefreshToken;
        }

    }
}
