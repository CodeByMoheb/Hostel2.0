using System.ComponentModel.DataAnnotations;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models.ViewModels
{
    public class UserEditViewModel
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Role")]
        public UserRole? Role { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Hostel")]
        public string? HostelId { get; set; }

        // Navigation properties for dropdowns
        public IEnumerable<Hostel> Hostels { get; set; } = new List<Hostel>();
    }

    public class ProfileViewModel
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public required string LastName { get; set; }

        [Display(Name = "Email")]
        public required string Email { get; set; }

        [Display(Name = "Role")]
        public UserRole? Role { get; set; }

        [Display(Name = "Hostel")]
        public string? HostelName { get; set; }

        [Display(Name = "Room Number")]
        public string? RoomNumber { get; set; }
    }
} 