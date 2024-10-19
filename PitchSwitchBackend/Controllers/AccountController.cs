using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PitchSwitchBackend.Dtos.Account.Requests;
using PitchSwitchBackend.Models;
using PitchSwitchBackend.Services.AuthService;
using PitchSwitchBackend.Services.TokenService;

namespace PitchSwitchBackend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager,
            IAuthService authService,
            ITokenService tokenService,
            ILogger<AccountController> logger
            )
        {
            _authService = authService;
            _tokenService = tokenService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var registerResult = await _authService.RegisterUser(registerDto);

                if (registerResult.IsSuccess)
                {
                    return Ok(registerResult.Data);
                }

                return BadRequest(registerResult.Errors);
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var loginResult = await _authService.LoginUser(loginDto);
                if (loginResult.IsSuccess)
                {
                    return Ok(loginResult.Data);
                }
                else
                {
                    return Unauthorized(loginResult.ErrorMessage);
                }
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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var refreshTokenResult = await _authService.RefreshToken(refreshTokenRequestDto);

                if (refreshTokenResult.IsSuccess)
                {
                    return Ok(refreshTokenResult.Data);
                }

                return Unauthorized(new { Message = refreshTokenResult.ErrorMessage, ModelState });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogError($"Refresh token is expired:\n{ex.Message}");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown during refreshing the token:\n{ex.Message}");
                return StatusCode(500, "An internal server error occurred while processing the request");
            }
        }
    }
}
