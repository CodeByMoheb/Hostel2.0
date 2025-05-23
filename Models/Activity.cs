using System;

namespace Hostel2._0.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public int HostelId { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }

        public Hostel Hostel { get; set; } = null!;
    }
} 