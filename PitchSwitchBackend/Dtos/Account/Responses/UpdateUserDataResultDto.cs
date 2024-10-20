namespace PitchSwitchBackend.Dtos.Account.Responses
{
    public class UpdateUserDataResultDto
    {
        public string UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? FavouriteClubId { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
    }
}
