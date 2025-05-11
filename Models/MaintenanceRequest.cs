using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models
{
    public class MaintenanceRequest : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [ForeignKey("Hostel")]
        public int HostelId { get; set; }
        public Hostel? Hostel { get; set; }

        [ForeignKey("Room")]
        public int? RoomId { get; set; }
        public Room? Room { get; set; }

        [ForeignKey("Student")]
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        [Required]
        public MaintenancePriority Priority { get; set; }
        
        [Required]
        public MaintenanceCategory Category { get; set; }

        [Required]
        public MaintenanceStatus Status { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public DateTime? CompletedDate { get; set; }
        
        // Alias for views that expect the property with a different name
        public DateTime? CompletionDate => CompletedDate;

        [StringLength(500)]
        public string? ResolutionNotes { get; set; }

        [StringLength(450)]
        public string? AssignedTo { get; set; }

        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [StringLength(450)]
        public string? UpdatedBy { get; set; }

        [Required]
        public string IssueType { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string? Notes { get; set; }
    }
} 