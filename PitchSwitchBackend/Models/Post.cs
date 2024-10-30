using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Content { get; set; }
        [StringLength(255)]
        public string? ImageUrl { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        [Required]
        public bool IsEdited { get; set; } = false;
        [Required]
        public string CreatedByUserId { get; set; }
        public AppUser CreatedByUser { get; set; }
        public int? TransferId { get; set; }
        public Transfer? Transfer { get; set; }
        public int? TransferRumourId { get; set; }
        public TransferRumour? TransferRumour { get; set; }
    }
}
