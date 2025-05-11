using System.ComponentModel.DataAnnotations;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models.ViewModels
{
    public class HostelCreateViewModel
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Hostel Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [Phone]
        [Display(Name = "Contact Number")]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class HostelApprovalViewModel
    {
        public Hostel Hostel { get; set; } = null!;
        public ApprovalStatus ApprovalStatus { get; set; }
        public string? Comments { get; set; }
    }

    public class ChooseSubscriptionViewModel
    {
        public int HostelId { get; set; }
        public string HostelName { get; set; } = string.Empty;
        public List<SubscriptionPlan> SubscriptionPlans { get; set; } = new List<SubscriptionPlan>();
        public SubscriptionBillingCycle BillingCycle { get; set; }
    }

    public class SubscriptionDetailsViewModel
    {
        public Hostel Hostel { get; set; } = null!;
        public HostelSubscription? Subscription { get; set; }
    }

    public class JoinHostelViewModel
    {
        [Required]
        [StringLength(8)]
        [Display(Name = "Hostel Registration Code")]
        public string RegistrationCode { get; set; } = string.Empty;
    }
} 