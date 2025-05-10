using System.ComponentModel.DataAnnotations;

namespace Hostel2._0.Models
{
    public class Hostel : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(200)]
        public required string Address { get; set; }

        [Required]
        [StringLength(100)]
        public required string City { get; set; }

        [Required]
        [StringLength(100)]
        public required string State { get; set; }

        [Required]
        [StringLength(20)]
        public required string PostalCode { get; set; }

        [Required]
        [Phone]
        public required string ContactNumber { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(20)]
        public required string RegistrationCode { get; set; }

        public bool IsActive { get; set; }
        public DateTime SubscriptionStartDate { get; set; }
        public DateTime SubscriptionEndDate { get; set; }
        public decimal MonthlyFee { get; set; }
        
        // Navigation properties
        public string? ManagerId { get; set; }
        public virtual ApplicationUser? Manager { get; set; }
        public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();
        public virtual ICollection<Notice> Notices { get; set; } = new List<Notice>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
} 