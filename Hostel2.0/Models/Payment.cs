using System.ComponentModel.DataAnnotations;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models
{
    public class Payment : BaseEntity
    {
        [Required]
        [Range(0, 100000)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentType Type { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }

        [StringLength(100)]
        public string? TransactionId { get; set; }

        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
        
        // Navigation properties
        [Required]
        public required string UserId { get; set; }
        public virtual required ApplicationUser User { get; set; }

        [Required]
        public required string HostelId { get; set; }
        public virtual required Hostel Hostel { get; set; }
    }
} 