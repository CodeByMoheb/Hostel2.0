using Hostel2._0.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hostel2._0.Models
{
    public class HostelApproval : BaseEntity
    {
        [Required]
        public int HostelId { get; set; }
        
        [ForeignKey("HostelId")]
        public virtual Hostel? Hostel { get; set; }
        
        [Required]
        public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;
        
        public string? AdminId { get; set; }
        public virtual ApplicationUser? Admin { get; set; }
        
        public DateTime? ApprovedAt { get; set; }
        
        [StringLength(500)]
        public string? Comments { get; set; }
        
        public bool IsVerified { get; set; }
    }
} 