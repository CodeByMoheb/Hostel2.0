using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models
{
    public class Payment : BaseEntity
    {
        [Required]
        [ForeignKey("Hostel")]
        public int HostelId { get; set; }
        public Hostel? Hostel { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        [Required]
        [StringLength(100)]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }
        
        // Aliases for views that expect different property names
        public DateTime PaidDate => PaymentDate;
        public DateTime PaidAt => PaymentDate;
        
        public DateTime? DueDate { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }

        [Required]
        public PaymentMethod Method { get; set; }
        
        // Type property for views
        public PaymentType Type { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(200)]
        public string? PaymentProof { get; set; }

        [ForeignKey("Room")]
        public int? RoomId { get; set; }
        public Room? Room { get; set; }
        
        // User property for views
        [NotMapped]
        public ApplicationUser? User => Student?.User;

        [StringLength(100)]
        public string? SubscriptionPlan { get; set; }

        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [StringLength(450)]
        public string? UpdatedBy { get; set; }
    }
} 