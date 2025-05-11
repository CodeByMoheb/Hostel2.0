using Hostel2._0.Models;

namespace Hostel2._0.Models.ViewModels.DashboardViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalHostels { get; set; }
        public int ActiveHostels { get; set; }
        public int PendingApprovals { get; set; }
        public ICollection<HostelSubscription> RecentSubscriptions { get; set; }

        public AdminDashboardViewModel()
        {
            RecentSubscriptions = new List<HostelSubscription>();
        }
    }
} 