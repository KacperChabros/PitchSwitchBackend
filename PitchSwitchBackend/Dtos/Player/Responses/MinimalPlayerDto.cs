using PitchSwitchBackend.Dtos.Club.Responses;

namespace PitchSwitchBackend.Dtos.Player.Responses
{
    public class MinimalPlayerDto
    {
        public int PlayerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; }
        public string Position { get; set; }
        public decimal MarketValue { get; set; }
        public string? PhotoUrl { get; set; }
        public MinimalClubDto? Club { get; set; }
    }
}
