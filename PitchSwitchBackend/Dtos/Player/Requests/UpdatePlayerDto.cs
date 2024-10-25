using PitchSwitchBackend.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Player.Requests
{
    public class UpdatePlayerDto
    {
        [StringLength(100, MinimumLength = 2)]
        public string? FirstName { get; set; } = null;
        [StringLength(100, MinimumLength = 2)]
        public string? LastName { get; set; } = null;
        public DateTime? DateOfBirth { get; set; } = null;
        [StringLength(100, MinimumLength = 2)]
        public string? Nationality { get; set; } = null;
        [StringLength(100, MinimumLength = 2)]
        public string? Position { get; set; } = null;
        [Range(100, 230)]
        public int? Height { get; set; } = null; // centimeters
        [Range(30, 200)]
        public int? Weight { get; set; } = null; // kilograms
        public Foot? PreferredFoot { get; set; } = null;
        [Range(0, 1000000000)]
        public decimal? MarketValue { get; set; } = null;
        [StringLength(255, MinimumLength = 2)]
        public string? PhotoUrl { get; set; } = null;
        [Required]
        [DefaultValue(false)]
        public bool IsPhotoUrlDeleted { get; set; } = false;
        public int? ClubId { get; set; } = null;
        [Required]
        [DefaultValue(false)]
        public bool IsClubIdDeleted { get; set; } = false;
    }
}
