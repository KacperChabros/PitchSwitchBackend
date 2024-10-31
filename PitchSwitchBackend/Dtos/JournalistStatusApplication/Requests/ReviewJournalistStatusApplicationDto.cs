using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.JournalistStatusApplication.Requests
{
    public class ReviewJournalistStatusApplicationDto
    {
        [Required]
        public bool IsAccepted { get; set; } = false;
        [StringLength(500, MinimumLength = 2)]
        public string? RejectionReason { get; set; }
    }
}
