using PitchSwitchBackend.Dtos.Account.Responses;

namespace PitchSwitchBackend.Dtos.Post.Responses
{
    public class MinimalPostDto
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public MinimalUserDto CreatedByUser { get; set; }
    }
}
