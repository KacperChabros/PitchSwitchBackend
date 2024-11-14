using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Club.Requests
{
    public class UpdateClubDto
    {
        [StringLength(255, MinimumLength = 3)]
        public string? Name { get; set; } = null;
        [StringLength(5, MinimumLength = 2)]
        public string? ShortName { get; set; } = null;
        [StringLength(150, MinimumLength = 3)]
        public string? League { get; set; } = null;
        [StringLength(150, MinimumLength = 3)]
        public string? Country { get; set; } = null;
        [StringLength(150, MinimumLength = 3)]
        public string? City { get; set; } = null;
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Foundation Year must be a four-digit number.")]
        public int? FoundationYear { get; set; } = null;
        [StringLength(255, MinimumLength = 3)]
        public string? Stadium { get; set; } = null;
        public IFormFile? Logo { get; set; } = null;
        [Required]
        [DefaultValue(false)]
        public bool IsLogoDeleted { get; set; } = false;
    }
}
