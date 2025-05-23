using System;

namespace Hostel2._0.Models
{
    public class AttendanceRecord
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public int? RoomId { get; set; }

        public Student Student { get; set; } = null!;
        public Room? Room { get; set; }
    }
} 