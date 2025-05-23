using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Address { get; set; }

        public override string? PhoneNumber { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? PostalCode { get; set; }

        [StringLength(50)]
        public string? Country { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginDate { get; set; }

        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [StringLength(450)]
        public string? UpdatedBy { get; set; }

        public string? ProfilePicture { get; set; }

        // For students
        [StringLength(20)]
        public string? StudentId { get; set; }
        
        [StringLength(100)]
        public string? University { get; set; }
        
        [StringLength(100)]
        public string? Course { get; set; }
        
        [StringLength(50)]
        public string? EmergencyContactName { get; set; }
        
        [StringLength(20)]
        public string? EmergencyContactPhone { get; set; }
        
        [StringLength(50)]
        public string? EmergencyContactRelationship { get; set; }

        // For hostel managers
        public bool IsHostelManager { get; set; }
        public string? ManagerLicenseNumber { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        
        // Student hostel/room assignment
        public int? HostelId { get; set; }
        
        [ForeignKey("HostelId")]
        public virtual Hostel? Hostel { get; set; }
        
        public int? RoomId { get; set; }
        
        [ForeignKey("RoomId")]
        public virtual Room? Room { get; set; }

        [Required]
        public UserRole Role { get; set; }

        // Navigation properties
        public virtual ICollection<Hostel> ManagedHostels { get; set; } = new HashSet<Hostel>();

        // Audit fields
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public ApplicationUser()
        {
            ManagedHostels = new HashSet<Hostel>();
            CreatedAt = DateTime.UtcNow;
        }

        public string Name => $"{FirstName} {LastName}";
    }
} 