using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required]
        [StringLength(300, MinimumLength = 3)]
        public string Content { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        [Required]
        public bool IsEdited { get; set; } = false;
        [Required]
        public string CreatedByUserId { get; set; }
        public AppUser CreatedByUser { get; set; }
        [Required]
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
