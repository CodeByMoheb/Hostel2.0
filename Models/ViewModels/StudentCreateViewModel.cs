using System;
using System.ComponentModel.DataAnnotations;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models.ViewModels
{
    public class StudentCreateViewModel
    {
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
        
        [Required]
        [StringLength(50)]
        public string StudentId { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;

        // Generated Fields
        public string GeneratedUsername { get; set; } = string.Empty;
        public string GeneratedPassword { get; set; } = string.Empty;
    }
} 