using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Comment.Requests
{
    public class UpdateCommentDto
    {
        [StringLength(300, MinimumLength = 3)]
        public string? Content { get; set; } = null;
    }
}
