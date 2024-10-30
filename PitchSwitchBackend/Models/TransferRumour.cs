using PitchSwitchBackend.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PitchSwitchBackend.Models
{
    public class TransferRumour
    {
        [Key]
        public int TransferRumourId { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public TransferType TransferType { get; set; }
        [Required]
        [Range(0, 1000000000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal RumouredFee { get; set; }
        [Required]
        [Range(1,10)]
        public int ConfidenceLevel {  get; set; }
        public bool IsConfirmed { get; set; } = false;
        public bool IsArchived { get; set; } = false;
        [Required]
        public string CreatedByUserId { get; set; }
        public AppUser CreatedByUser { get; set; }
        [Required]
        public int PlayerId { get; set; }
        [Required]
        public Player Player { get; set; }
        public int? SellingClubId { get; set; }
        public Club? SellingClub { get; set; }
        public int? BuyingClubId { get; set; }
        public Club? BuyingClub { get; set; }
        public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
    }
}
