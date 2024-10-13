using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [ProtectedPersonalData]
        public override string? UserName { get; set ; }
        [Required]
        [ProtectedPersonalData]
        public override string? Email { get; set; }
        [Required]
        [ProtectedPersonalData]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [ProtectedPersonalData]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public int? FavouriteClubId { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
    }
}
