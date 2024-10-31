using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Models
{
    public class JournalistStatusApplication
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 2)]
        public string Motivation {  get; set; }
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        [Required]
        public bool IsAccepted { get; set; } = false;
        [Required]
        public bool IsReviewed { get; set; } = false;
        public DateTime? ReviewedOn { get; set; }
        [StringLength(500, MinimumLength = 2)]
        public string? RejectionReason { get; set; }
        [Required]
        public string SubmittedByUserId { get; set; }
        public AppUser SubmittedByUser { get; set; }
    }
}
