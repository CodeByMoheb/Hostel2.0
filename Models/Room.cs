using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models
{
    public class Room : BaseEntity
    {
        public Room()
        {
            Students = new List<ApplicationUser>();
            MaintenanceRequests = new List<MaintenanceRequest>();
        }

        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;

        [Required]
        public RoomType Type { get; set; }

        [Required]
        [StringLength(50)]
        public string Floor { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Block { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public int Capacity { get; set; }
        public int CurrentOccupancy { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MonthlyRent { get; set; }
        
        public bool IsAvailable => CurrentOccupancy < Capacity;

        [Required]
        [ForeignKey("Hostel")]
        public int HostelId { get; set; }
        public Hostel? Hostel { get; set; }

        public ICollection<ApplicationUser> Students { get; set; }
        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }

        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [StringLength(450)]
        public string? UpdatedBy { get; set; }
    }
} 