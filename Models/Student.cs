using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Hostel2._0.Models.MealManagement;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models
{
    public class Student : BaseEntity
    {
        public Student()
        {
            Payments = new List<Payment>();
            MaintenanceRequests = new List<MaintenanceRequest>();
            StudentMeals = new List<StudentMeal>();
            Documents = new List<Document>();
        }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [StringLength(200)]
        public string? Address { get; set; }
        
        [ForeignKey("Room")]
        public int? RoomId { get; set; }
        public Room? Room { get; set; }
        
        [ForeignKey("Hostel")]
        public int HostelId { get; set; }
        public Hostel? Hostel { get; set; }
        
        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsApproved { get; set; }
        public bool IsManagerCreated { get; set; }
        public string? RegistrationCode { get; set; }
        public new DateTime CreatedAt { get; set; } = DateTime.Now;
        public new DateTime? UpdatedAt { get; set; }

        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [StringLength(450)]
        public string? UpdatedBy { get; set; }
        
        public string FullName => $"{FirstName} {LastName}";
        
        public ICollection<Payment> Payments { get; set; }
        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
        public ICollection<StudentMeal> StudentMeals { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
} 