using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hostel2._0.Models.MealManagement;

namespace Hostel2._0.Models
{
    public class Hostel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [StringLength(450)]
        public string ManagerId { get; set; } = string.Empty;
        public ApplicationUser? Manager { get; set; }

        [Required]
        [StringLength(8)]
        public string JoinCode { get; set; } = string.Empty;

        public bool IsActive { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionEndDate { get; set; }
        public string SubscriptionPlan { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyFee { get; set; }

        // Navigation properties
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Notice> Notices { get; set; }
        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Payment> Payments { get; set; }
        public ICollection<Meal> Meals { get; set; }
        public ICollection<MealPlan> MealPlans { get; set; }
        public ICollection<HostelSubscription> Subscriptions { get; set; }

        // Amenities and facilities
        public bool HasWifi { get; set; }
        public bool HasLaundry { get; set; }
        public bool HasCafeteria { get; set; }
        public bool HasGym { get; set; }
        public bool HasStudyRoom { get; set; }
        public bool HasSecurity { get; set; }

        // Rules and policies
        [StringLength(1000)]
        public string? CheckInTime { get; set; }
        [StringLength(1000)]
        public string? CheckOutTime { get; set; }
        [StringLength(1000)]
        public string? GuestPolicy { get; set; }
        [StringLength(1000)]
        public string? NoisePolicy { get; set; }

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(100)]
        public string? City { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? PostalCode { get; set; }

        [StringLength(20)]
        public string? ContactNumber { get; set; }

        [StringLength(50)]
        public string? RegistrationCode { get; set; }

        public Hostel()
        {
            Rooms = new HashSet<Room>();
            Notices = new HashSet<Notice>();
            MaintenanceRequests = new HashSet<MaintenanceRequest>();
            Documents = new HashSet<Document>();
            Students = new HashSet<Student>();
            Payments = new HashSet<Payment>();
            Meals = new HashSet<Meal>();
            MealPlans = new HashSet<MealPlan>();
            Subscriptions = new HashSet<HostelSubscription>();
            CreatedAt = DateTime.UtcNow;
            IsActive = false; // Requires admin approval
        }
    }
} 