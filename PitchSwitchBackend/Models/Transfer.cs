using PitchSwitchBackend.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitchSwitchBackend.Models
{
    public class Transfer
    {
        [Key]
        public int TransferId { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(30)")]
        public TransferType TransferType { get; set; }
        [Required]
        public DateTime TransferDate { get; set; }
        [Required]
        [Range(0, 1000000000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TransferFee { get; set; }
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
