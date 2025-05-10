using System.ComponentModel.DataAnnotations;

namespace Hostel2._0.Models
{
    public class Notice : BaseEntity
    {
        [Required]
        [StringLength(200)]
        public required string Title { get; set; }

        [Required]
        public required string Content { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public bool IsImportant { get; set; }
        
        // Navigation properties
        [Required]
        public required string HostelId { get; set; }
        public virtual required Hostel Hostel { get; set; }

        [Required]
        public required string CreatedById { get; set; }
        public virtual required ApplicationUser CreatedBy { get; set; }
    }
} 