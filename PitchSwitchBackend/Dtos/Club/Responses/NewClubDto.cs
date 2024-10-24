namespace PitchSwitchBackend.Dtos.Club.Responses
{
    public class NewClubDto
    {
        public int ClubId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string League { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int FoundationYear { get; set; }
        public string Stadium { get; set; }
        public string? LogoUrl { get; set; }
    }
}
