using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Images.Requests
{
    public class DeleteImageDto
    {
        [Required]
        [StringLength(255, MinimumLength = 2)]
        public string ImageUrl { get; set; }
    }
}
