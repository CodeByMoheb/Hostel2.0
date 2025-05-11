using System.ComponentModel.DataAnnotations;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models.ViewModels
{
    public class UserEditViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Role")]
        public UserRole? Role { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Hostel")]
        public int? HostelId { get; set; }

        // Navigation property for dropdown
        public IEnumerable<Hostel> Hostels { get; set; } = new List<Hostel>();
    }

    public class ProfileViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public UserRole Role { get; set; }

        [Display(Name = "Hostel")]
        public string HostelName { get; set; } = string.Empty;

        [Display(Name = "Room")]
        public string RoomNumber { get; set; } = string.Empty;
    }
} 