using PitchSwitchBackend.Dtos.Club.Responses;
using PitchSwitchBackend.Dtos.Post.Responses;

namespace PitchSwitchBackend.Dtos.Account.Responses
{
    public class GetUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public MinimalClubDto? FavouriteClub { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Bio { get; set; }
        public IEnumerable<ListElementPostDto> Posts { get; set; } = new List<ListElementPostDto>();
    }
}
