using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.JournalistStatusApplication.Requests
{
    public class UpdateJournalistStatusApplicationDto
    {
        [StringLength(500, MinimumLength = 2)]
        public string Motivation { get; set; }
    }
}
