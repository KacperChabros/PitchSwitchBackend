using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PitchSwitchBackend.Data;
using PitchSwitchBackend.Dtos.Account;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.TokenService;

namespace PitchSwitchBackend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDBContext _context;
        private readonly ILogger<AccountController> _logger;
        public AccountController(UserManager<AppUser> userManager,
            ITokenService tokenService,
            SignInManager<AppUser> signInManager,
            ApplicationDBContext context,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var appUser = new AppUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    FavouriteClubId = registerDto.FavouriteClub,
                    ProfilePictureUrl = registerDto.ProfilePictureUrl,
                    Bio = registerDto.Bio
                };

                var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        return Ok(
                            new NewUserDto
                            {
                                UserName = appUser.UserName,
                                Email = appUser.Email,
                                FirstName = appUser.FirstName,
                                LastName = appUser.LastName,
                                FavouriteClub = appUser.FavouriteClubId,
                                ProfilePictureUrl = appUser.ProfilePictureUrl,
                                Bio = appUser.Bio,
                                Tokens = new TokensDto
                                {
                                    AccessToken = await _tokenService.CreateAccessToken(appUser),
                                    RefreshToken = await _tokenService.CreateRefreshToken(appUser)
                                }
                            }
                        );
                    }
                    else
                    {
                        return StatusCode(500, roleResult.Errors);
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error during updating the database in register endpoint:\n{ex.Message}");
                return StatusCode(500, "An internal server error occurred while processing the request");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown during registering user:\n{ex.Message}");
                return StatusCode(500, "An internal server error occurred while processing the request");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName.Equals(loginDto.Username.ToLower()));
                if (user == null)
                {
                    return Unauthorized("Username and/or password is invalid");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (!result.Succeeded)
                {
                    return Unauthorized("Username and/or password is invalid");
                }

                return Ok(
                    new NewUserDto
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        FavouriteClub = user.FavouriteClubId,
                        ProfilePictureUrl = user.ProfilePictureUrl,
                        Bio = user.Bio,
                        Tokens = new TokensDto
                        {
                            AccessToken = await _tokenService.CreateAccessToken(user),
                            RefreshToken = await _tokenService.CreateRefreshToken(user)
                        }
                    }
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error during updating the database in login endpoint:\n{ex.Message}");
                return StatusCode(500, "An internal server error occurred while processing the request");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown during logging in user:\n{ex.Message}");
                return StatusCode(500, "An internal server error occurred while processing the request");
            }
        }

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequestDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.Equals(refreshTokenRequestDto.UserName));

                if (user == null)
                {
                    return Unauthorized(ModelState);
                }

                var tokens = await _tokenService.RefreshAccessToken(user, refreshTokenRequestDto.RefreshToken);

                if (tokens == null)
                {
                    return StatusCode(500, "Refreshing the token failed");
                }

                return Ok(tokens);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"Refresh token is expired:\n{ex.Message}");
                return Unauthorized(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError($"Error during updating the database in refreshtoken endpoint:\n{ex.Message}");
                return StatusCode(500, "An internal server error occurred while processing the request");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown during refreshing the token:\n{ex.Message}");
                return StatusCode(500, "An internal server error occurred while processing the request");
            }
        }
    }
}
