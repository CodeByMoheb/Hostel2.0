using Hostel2._0.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Hostel2._0.Models
{
    public class HostelApproval : BaseEntity
    {
        [Required]
        public string HostelId { get; set; } = null!;
        public virtual Hostel Hostel { get; set; } = null!;
        
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