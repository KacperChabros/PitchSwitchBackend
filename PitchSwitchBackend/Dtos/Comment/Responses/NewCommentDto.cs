using PitchSwitchBackend.Dtos.Account.Responses;
using PitchSwitchBackend.Dtos.Post.Responses;

namespace PitchSwitchBackend.Dtos.Comment.Responses
{
    public class NewCommentDto
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsEdited { get; set; }
        public MinimalUserDto CreatedByUser { get; set; }
        public MinimalPostDto Post { get; set; }
    }
}
