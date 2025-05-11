using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models
{
    public class HostelSubscription : BaseEntity
    {
        [Required]
        [ForeignKey("Hostel")]
        public int HostelId { get; set; }
        public Hostel? Hostel { get; set; }

        [Required]
        [ForeignKey("SubscriptionPlan")]
        public int SubscriptionPlanId { get; set; }
        public SubscriptionPlan? SubscriptionPlan { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public SubscriptionStatus Status { get; set; }

        public DateTime? LastPaymentDate { get; set; }

        public DateTime? NextPaymentDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [StringLength(450)]
        public string? UpdatedBy { get; set; }
        
        // Additional properties for subscription functionality
        public string? TransactionId { get; set; }
        public PaymentMethod? PaymentMethod { get; set; }
        public string? InvoiceId { get; set; }
        public BillingCycle BillingCycle { get; set; }
    }
} 