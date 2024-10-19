using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Models
{
    public class RefreshToken
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string RefreshTokenHash { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Required]
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public RefreshToken()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
