using Hostel2._0.Models;

namespace Hostel2._0.Models.ViewModels.DashboardViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalHostels { get; set; }
        public int TotalStudents { get; set; }
        public int TotalRooms { get; set; }
        public int ActiveHostels { get; set; }
        public int PendingApprovals { get; set; }
        public int TotalManagers { get; set; }
        public List<Hostel> RecentHostels { get; set; } = new List<Hostel>();
        public List<ApplicationUser> RecentManagers { get; set; } = new List<ApplicationUser>();
    }
} 