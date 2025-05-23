using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models
{
    public class Hostel : BaseEntity
    {
        public Hostel()
        {
            Rooms = new List<Room>();
            Students = new List<Student>();
            Notices = new List<Notice>();
        }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string State { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

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
        [StringLength(20)]
        public string JoinCode { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        public string? RejectionReason { get; set; }

        public ICollection<Room> Rooms { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Notice> Notices { get; set; }

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
        public new DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        [StringLength(20)]
        public string? ContactNumber { get; set; }

        [StringLength(50)]
        public string? RegistrationCode { get; set; }

        // Subscription properties
        public decimal MonthlyFee { get; set; }
    }
} 