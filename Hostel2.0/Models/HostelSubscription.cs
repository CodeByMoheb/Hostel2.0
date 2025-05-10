using System.ComponentModel.DataAnnotations;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models
{
    public class HostelSubscription : BaseEntity
    {
        [Required]
        public string HostelId { get; set; } = null!;
        public virtual Hostel Hostel { get; set; } = null!;
        
        [Required]
        public string SubscriptionPlanId { get; set; } = null!;
        public virtual SubscriptionPlan SubscriptionPlan { get; set; } = null!;
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public SubscriptionBillingCycle BillingCycle { get; set; }
        
        [Required]
        public SubscriptionStatus Status { get; set; }
        
        public string? TransactionId { get; set; }
        public string? PaymentMethod { get; set; }
        public string? InvoiceId { get; set; }
        public string? Notes { get; set; }
    }
} 