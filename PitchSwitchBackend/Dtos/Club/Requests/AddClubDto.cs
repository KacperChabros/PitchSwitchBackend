using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Club.Requests
{
    public class AddClubDto
    {
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Name { get; set; }
        [Required]
        [StringLength(5, MinimumLength = 2)]
        public string ShortName { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string League { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Country { get; set; }
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string City { get; set; }
        [Required]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Foundation Year must be a four-digit number.")]
        public int FoundationYear { get; set; }
        [Required]
        [StringLength(255, MinimumLength = 3)]
        public string Stadium { get; set; }
        public IFormFile? Logo { get; set; } = null;
    }
}
