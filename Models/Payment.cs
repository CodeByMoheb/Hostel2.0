using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models
{
    public class Payment : BaseEntity
    {
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentStatus Status { get; set; }

        [Required]
        public PaymentType Type { get; set; }

        [Required]
        [ForeignKey("Hostel")]
        public int HostelId { get; set; }
        public Hostel? Hostel { get; set; }

        [ForeignKey("Student")]
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        public string? TransactionId { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? DueDate { get; set; }

        [Required]
        [StringLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty;

        [Required]
        public DateTime PaymentDate { get; set; }
        
        // Aliases for views that expect different property names
        public DateTime PaidDate => PaymentDate;
        
        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(200)]
        public string? PaymentProof { get; set; }

        [ForeignKey("Room")]
        public int? RoomId { get; set; }
        public Room? Room { get; set; }
        
        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [StringLength(450)]
        public string? UpdatedBy { get; set; }
    }
} 