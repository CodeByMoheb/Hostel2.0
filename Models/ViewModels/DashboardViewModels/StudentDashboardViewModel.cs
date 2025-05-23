using Hostel2._0.Models;
using System.Collections.Generic;

namespace Hostel2._0.Models.ViewModels.DashboardViewModels
{
    public class StudentDashboardViewModel
    {
        public Student Student { get; set; } = null!;
        public Room? Room { get; set; }
        public bool IsApproved { get; set; }
        public bool HasRoom { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal PendingPaymentAmount { get; set; }
        public int AttendancePercentage { get; set; }
        public int PendingMaintenanceRequests { get; set; }
        public ICollection<Notice> RecentNotices { get; set; } = new List<Notice>();
        public ICollection<Payment> RecentPayments { get; set; } = new List<Payment>();
        public ICollection<MaintenanceRequest> RecentMaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
    }
} 