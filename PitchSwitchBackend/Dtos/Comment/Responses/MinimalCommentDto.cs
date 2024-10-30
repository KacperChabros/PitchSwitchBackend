using PitchSwitchBackend.Dtos.Account.Responses;

namespace PitchSwitchBackend.Dtos.Comment.Responses
{
    public class MinimalCommentDto
    {
        public int CommentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsEdited { get; set; }
        public MinimalUserDto CreatedByUser { get; set; }
    }
}
