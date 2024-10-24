using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Images.Requests
{
    public class ImageUploadDto
    {
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string EntityType { get; set; }
        [Required]
        public IFormFile FormFile { get; set; }
    }
}
