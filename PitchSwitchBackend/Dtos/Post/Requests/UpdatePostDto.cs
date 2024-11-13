using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Post.Requests
{
    public class UpdatePostDto
    {
        [StringLength(100, MinimumLength = 3)]
        public string? Title { get; set; } = null;
        [StringLength(500, MinimumLength = 5)]
        public string? Content { get; set; } = null;
        public IFormFile? Image { get; set; } = null;
        [Required]
        [DefaultValue(false)]
        public bool IsImageDeleted { get; set; } = false;
        public int? TransferId { get; set; } = null;
        [Required]
        [DefaultValue(false)]
        public bool IsTransferDeleted { get; set; } = false;
        public int? TransferRumourId { get; set; } = null;
        [Required]
        [DefaultValue(false)]
        public bool IsTransferRumourDeleted { get; set; } = false;
    }
}
