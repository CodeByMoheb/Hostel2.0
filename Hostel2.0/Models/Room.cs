using System.ComponentModel.DataAnnotations;

namespace Hostel2._0.Models
{
    public class Room : BaseEntity
    {
        [Required]
        [StringLength(20)]
        public required string RoomNumber { get; set; }

        [Required]
        [StringLength(50)]
        public required string RoomType { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal MonthlyRent { get; set; }

        [Required]
        [Range(1, 10)]
        public int Capacity { get; set; }

        public int CurrentOccupancy { get; set; }
        
        public bool IsAvailable { get; set; }

        [Required]
        [StringLength(500)]
        public required string Description { get; set; }
        
        // Navigation properties
        [Required]
        public required string HostelId { get; set; }
        public virtual required Hostel Hostel { get; set; }
        public virtual ICollection<ApplicationUser> Occupants { get; set; } = new List<ApplicationUser>();
    }
} 