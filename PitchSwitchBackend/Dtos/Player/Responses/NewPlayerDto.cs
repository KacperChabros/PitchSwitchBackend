using PitchSwitchBackend.Dtos.Club.Responses;
using PitchSwitchBackend.Enums;

namespace PitchSwitchBackend.Dtos.Player.Responses
{
    public class NewPlayerDto
    {
        public int PlayerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; }
        public string Position { get; set; }
        public int Height { get; set; } // centimeters
        public int Weight { get; set; } // kilograms
        public Foot PreferredFoot { get; set; }
        public decimal MarketValue { get; set; }
        public string? PhotoUrl { get; set; }
        public ClubDto? Club { get; set; }
    }
}
