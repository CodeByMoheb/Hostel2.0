using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public required string LastName { get; set; }

        [Required]
        public UserRole Role { get; set; }

        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        
        // Navigation properties
        public string? HostelId { get; set; }
        public virtual Hostel? Hostel { get; set; }
        public string? RoomId { get; set; }
        public virtual Room? Room { get; set; }
    }
} 