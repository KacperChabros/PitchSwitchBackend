using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [ProtectedPersonalData]
        [StringLength(70)]
        public override string? UserName { get; set ; }
        [Required]
        [ProtectedPersonalData]
        [StringLength(255)]
        public override string? Email { get; set; }
        [Required]
        [ProtectedPersonalData]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [ProtectedPersonalData]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        [StringLength(255)]
        public string? ProfilePictureUrl { get; set; }
        [StringLength(500)]
        public string? Bio { get; set; }
        public int? FavouriteClubId { get; set; }
        public Club? FavouriteClub {  get; set; }
    }
}
