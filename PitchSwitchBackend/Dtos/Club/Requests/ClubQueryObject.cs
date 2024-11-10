using PitchSwitchBackend.Helpers;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Dtos.Club.Requests
{
    public class ClubQueryObject
    {
        [StringLength(255)]
        public string? Name { get; set; } = null;
        [StringLength(5)]
        public string? ShortName { get; set; } = null;
        [StringLength(150)]
        public string? League { get; set; } = null;
        [StringLength(150)]
        public string? Country { get; set; } = null;
        public bool? IncludeArchived { get; set; } = false;
        [StringLength(50)]
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;
        [Range(1, 1000)]
        public int PageNumber { get; set; } = 1;
        [Range(1, 50)]
        public int PageSize { get; set; } = 10;
    }
}
