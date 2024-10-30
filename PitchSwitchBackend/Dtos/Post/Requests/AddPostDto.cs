using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Post.Requests
{
    public class AddPostDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; }
        [Required]
        [StringLength(500, MinimumLength = 5)]
        public string Content { get; set; }
        [StringLength(255, MinimumLength = 3)]
        public string? ImageUrl { get; set; }
        public int? TransferId { get; set; }
        public int? TransferRumourId { get; set; }
    }
}
