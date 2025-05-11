using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hostel2._0.Models
{
    public class NoticeRecipient : BaseEntity
    {
        [Required]
        [ForeignKey("Notice")]
        public int NoticeId { get; set; }
        public Notice? Notice { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }

        public NoticeRecipient()
        {
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }
    }
} 