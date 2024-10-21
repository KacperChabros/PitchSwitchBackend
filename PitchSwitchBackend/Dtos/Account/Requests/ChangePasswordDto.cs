using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Account.Requests
{
    public class ChangePasswordDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
