using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PitchSwitchBackend.Dtos.Account.Requests;
using PitchSwitchBackend.Mappers;
using PitchSwitchBackend.Services.AuthService;
using System.Security.Claims;

namespace PitchSwitchBackend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(
            IAuthService authService
            )
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
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

            return BadRequest(new { Message = registerResult.ErrorMessage, registerResult.Errors });
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

        [HttpGet("getuserbyname/{userName}")]
        [Authorize]
        public async Task<IActionResult> GetUserByName([FromRoute] string userName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = await _authService.FindUserByName(userName);

            if (appUser == null)
            {
                return NotFound("There is no such user");
            }
            
            return Ok(appUser.FromModelToGetUserDto());
        }

        [HttpGet("getuserbynamewithdata/{userName}")]
        [Authorize]
        public async Task<IActionResult> GetUserByNameWithData([FromRoute] string userName)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var appUser = await _authService.FindUserByNameWithData(userName);

            if (appUser == null)
            {
                return NotFound("There is no such user");
            }

            return Ok(appUser.FromModelToGetUserDto());
        }

        [HttpPut("updateuserdata")]
        [Authorize]
        public async Task<IActionResult> UpdateUserData([FromForm] UpdateUserDataDto updateUserDataDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userName = User.FindFirstValue(ClaimTypes.GivenName);
            if (userName == null)
            {
                return Unauthorized("You are unauthorized");
            }

            var appUser = await _authService.FindUserByName(userName);
            if (appUser == null)
            {
                return NotFound("There is no such user");
            }

            var updateUserDataResult = await _authService.UpdateUserData(appUser, updateUserDataDto);

            if (updateUserDataResult.IsSuccess)
            {
                return Ok(updateUserDataResult.Data);
            }

            return BadRequest(new { Message = updateUserDataResult.ErrorMessage, updateUserDataResult.Errors, ModelState });
        }

        [HttpPut("changepassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userName = User.FindFirstValue(ClaimTypes.GivenName);
            if (userName == null)
            {
                return Unauthorized("You are unauthorized");
            }

            var appUser = await _authService.FindUserByName(userName);
            if (appUser == null)
            {
                return NotFound("There is no such user");
            }

            var changePasswordResult = await _authService.ChangePassword(appUser, changePasswordDto);

            if (changePasswordResult.IsSuccess)
            {
                return Ok("The password has been successfully changed");
            }

            return BadRequest(new { Message = changePasswordResult.ErrorMessage, changePasswordResult.Errors });
        }

        [HttpDelete("deleteuser")]
        public async Task<IActionResult> DeleteUser()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userName = User.FindFirstValue(ClaimTypes.GivenName);
            if (userName == null)
            {
                return Unauthorized("You are unauthorized");
            }

            var appUser = await _authService.FindUserByNameWithData(userName);
            if (appUser == null)
            {
                return NotFound("There is no such user");
            }

            var deleteUserResult = await _authService.DeleteUser(appUser);

            if (deleteUserResult.IsSuccess)
            {
                return NoContent();
            }

            return BadRequest(new { Message = deleteUserResult.ErrorMessage, deleteUserResult.Errors });
        }
    }
}
