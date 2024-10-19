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

        public AccountController(UserManager<AppUser> userManager,
            IAuthService authService,
            ITokenService tokenService
            )
        {
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var registerResult = await _authService.RegisterUser(registerDto);

            if (registerResult.IsSuccess)
            {
                return Ok(registerResult.Data);
            }

            return BadRequest(registerResult.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenRequestDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var refreshTokenResult = await _authService.RefreshToken(refreshTokenRequestDto);

            if (refreshTokenResult.IsSuccess)
            {
                return Ok(refreshTokenResult.Data);
            }

            return Unauthorized(new { Message = refreshTokenResult.ErrorMessage, ModelState });
        }
    }
}
