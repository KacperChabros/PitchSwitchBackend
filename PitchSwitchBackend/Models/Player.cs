using PitchSwitchBackend.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PitchSwitchBackend.Models
{
    public class Player
    {
        [Key]
        public int PlayerId { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [StringLength(100)]
        public string Nationality { get; set; }
        [Required]
        [StringLength(100)]
        public string Position { get; set; }
        [Required]
        [Range(100, 230)]
        public int Height { get; set; } // centimeters
        [Required]
        [Range(30, 200)]
        public int Weight { get; set; } // kilograms
        [Required]
        [Column(TypeName = "nvarchar(10)")]
        public Foot PreferredFoot { get; set; }
        [Required]
        [Range(0, 1000000000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MarketValue { get; set; }
        [StringLength(255)]
        public string? PhotoUrl { get; set; }
        public int? ClubId { get; set; }
        public Club? Club { get; set; }
        public virtual ICollection<Transfer> Transfers { get; set; } = new List<Transfer>();
        public virtual ICollection<TransferRumour> TransferRumours  { get; set; } = new List<TransferRumour>();
    }
}
