using PitchSwitchBackend.Enums;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Player.Requests
{
    public class AddPlayerDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Nationality { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Position { get; set; }
        [Required]
        [Range(100, 230)]
        public int Height { get; set; } // centimeters
        [Required]
        [Range(30, 200)]
        public int Weight { get; set; } // kilograms
        [Required]
        public Foot PreferredFoot { get; set; }
        [Required]
        [Range(0, 1000000000)]
        public decimal MarketValue { get; set; }
        [StringLength(255, MinimumLength = 2)]
        public string? PhotoUrl { get; set; }
        public int? ClubId { get; set; }
    }
}
