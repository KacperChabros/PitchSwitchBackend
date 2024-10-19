using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Account.Requests
{
    public class RefreshTokenRequestDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string UserName { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
