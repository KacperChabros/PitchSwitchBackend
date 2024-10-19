using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Account.Requests
{
    public class RegisterDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public string? Username { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string? FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        public string? LastName { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Favourite club must be a valid club ID.")]
        public int? FavouriteClub { get; set; }
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Profile Picture Url must be betwwen 2 and 200 characters.")]
        public string? ProfilePictureUrl { get; set; }
        [StringLength(500, ErrorMessage = "Bio cannot be longer than 500 characters.")]
        public string? Bio { get; set; }
    }
}
