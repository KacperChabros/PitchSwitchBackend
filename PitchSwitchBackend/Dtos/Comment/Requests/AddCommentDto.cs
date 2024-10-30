using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Comment.Requests
{
    public class AddCommentDto
    {
        [Required]
        [StringLength(300, MinimumLength = 3)]
        public string Content { get; set; }
        [Required]
        public int PostId { get; set; }
    }
}
