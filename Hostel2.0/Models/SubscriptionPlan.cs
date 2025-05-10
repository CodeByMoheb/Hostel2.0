using System.ComponentModel.DataAnnotations;

namespace Hostel2._0.Models
{
    public class SubscriptionPlan : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        public required string Description { get; set; }

        [Required]
        public decimal MonthlyPrice { get; set; }
        
        [Required]
        public decimal YearlyPrice { get; set; }

        [Required]
        public int MaxStudents { get; set; }
        
        [Required]
        public int MaxRooms { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Features
        public bool IncludesMealManagement { get; set; } = true;
        public bool IncludesPaymentManagement { get; set; } = true;
        public bool IncludesAttendanceTracking { get; set; } = true;
        public bool IncludesReporting { get; set; } = true;
        public bool IncludesAPIAccess { get; set; } = false;
        
        // Navigation properties
        public virtual ICollection<HostelSubscription> HostelSubscriptions { get; set; } = new List<HostelSubscription>();
    }
} 