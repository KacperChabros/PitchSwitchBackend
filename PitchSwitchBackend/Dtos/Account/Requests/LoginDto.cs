using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Account.Requests
{
    public class LoginDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
