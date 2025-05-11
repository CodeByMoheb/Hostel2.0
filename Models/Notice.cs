using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hostel2._0.Models
{
    public class Notice : BaseEntity
    {
        public Notice()
        {
            Recipients = new List<NoticeRecipient>();
        }

        [Required]
        [ForeignKey("Hostel")]
        public int HostelId { get; set; }
        public Hostel? Hostel { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime PublishDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; }

        public NoticePriority Priority { get; set; }

        public NoticeType Type { get; set; }

        public ICollection<NoticeRecipient> Recipients { get; set; }

        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [StringLength(450)]
        public string? UpdatedBy { get; set; }
    }

    public enum NoticePriority
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Urgent = 3
    }

    public enum NoticeType
    {
        General = 0,
        Maintenance = 1,
        Event = 2,
        Emergency = 3,
        Payment = 4
    }
} 