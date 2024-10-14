using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Account
{
    public class RefreshTokenRequestDto
    {
        [Required]
        public string RefreshToken { get; set; }
        [Required]
        public string UserName { get; set; }
    }
}
