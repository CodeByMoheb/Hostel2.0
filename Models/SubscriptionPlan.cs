using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Hostel2._0.Models
{
    public class SubscriptionPlan : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyPrice { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal YearlyPrice { get; set; }

        [Required]
        public int DurationInDays { get; set; }

        [Required]
        public int MaxRooms { get; set; }

        [Required]
        public int MaxStudents { get; set; }

        [Required]
        public bool IsActive { get; set; }

        // Feature flags
        public bool IncludesMealManagement { get; set; }
        public bool IncludesPaymentManagement { get; set; }
        public bool IncludesAttendanceTracking { get; set; }
        public bool IncludesReporting { get; set; }
        public bool IncludesAPIAccess { get; set; }

        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [StringLength(450)]
        public string? UpdatedBy { get; set; }

        public ICollection<HostelSubscription> Subscriptions { get; set; } = new List<HostelSubscription>();
    }
} 