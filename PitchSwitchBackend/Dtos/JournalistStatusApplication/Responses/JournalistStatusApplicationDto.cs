using PitchSwitchBackend.Dtos.Account.Responses;

namespace PitchSwitchBackend.Dtos.JournalistStatusApplication.Responses
{
    public class JournalistStatusApplicationDto
    {
        public int Id { get; set; }
        public string Motivation { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsReviewed { get; set; }
        public DateTime? ReviewedOn { get; set; }
        public string? RejectionReason { get; set; }
        public MinimalUserDto? SubmittedByUser { get; set; }
    }
}
