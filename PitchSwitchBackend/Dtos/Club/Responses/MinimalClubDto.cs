namespace PitchSwitchBackend.Dtos.Club.Responses
{
    public class MinimalClubDto
    {
        public int ClubId { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string? LogoUrl { get; set; }
    }
}
