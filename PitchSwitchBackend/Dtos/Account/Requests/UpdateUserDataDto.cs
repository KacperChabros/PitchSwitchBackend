using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Account.Requests
{
    public class UpdateUserDataDto
    {
        [EmailAddress]
        public string? Email { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string? FirstName { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters.")]
        public string? LastName { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Favourite club must be a valid club ID.")]
        public int? FavouriteClubId { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        [StringLength(500, ErrorMessage = "Bio cannot be longer than 500 characters.")]
        public string? Bio { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsBioDeleted { get; set; } = false;
        [Required]
        [DefaultValue(false)]
        public bool IsProfilePictureDeleted { get; set; } = false;
        [Required]
        [DefaultValue(false)]
        public bool IsFavouriteClubIdDeleted { get; set; } = false;
    }
}
