using System;
using System.Collections.Generic;
using Hostel2._0.Models;

namespace Hostel2._0.Models.ViewModels.DashboardViewModels
{
    public class ManagerDashboardViewModel
    {
        public Hostel Hostel { get; set; } = null!;
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int AvailableRooms { get; set; }
        public int TotalStudents { get; set; }
        public int ActiveNotices { get; set; }
        public decimal PendingPayments { get; set; }
        public decimal TotalPaymentsThisMonth { get; set; }
        public decimal TotalPendingPayments { get; set; }
        public int StudentsWithDues { get; set; }
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Notice> RecentNotices { get; set; } = new List<Notice>();
        public ICollection<Payment> RecentPayments { get; set; } = new List<Payment>();
        public ICollection<MaintenanceRequest> RecentMaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
        public List<ManagerDashboardStudentPaymentRow> AllStudentPayments { get; set; } = new List<ManagerDashboardStudentPaymentRow>();
    }

    public class Activity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }

    public class ManagerDashboardStudentPaymentRow
    {
        public Student Student { get; set; } = null!;
        public Room? Room { get; set; }
        public Payment? LatestPayment { get; set; }
    }
} 