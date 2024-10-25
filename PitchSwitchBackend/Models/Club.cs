using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Models
{
    public class Club
    {
        [Key]
        public int ClubId { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        [Required]
        [StringLength(5, MinimumLength = 2)]
        public string ShortName { get; set; }
        [Required]
        [StringLength(150)]
        public string League { get; set; }
        [Required]
        [StringLength(150)]
        public string Country { get; set; }
        [Required]
        [StringLength(150)]
        public string City { get; set; }
        [Required]
        public int FoundationYear { get; set; }
        [Required]
        [StringLength(255)]
        public string Stadium { get; set; }
        [StringLength(255)]
        public string? LogoUrl { get; set; }
    }
}
