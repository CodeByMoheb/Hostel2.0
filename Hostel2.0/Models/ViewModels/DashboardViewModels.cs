using System.ComponentModel.DataAnnotations;
using Hostel2._0.Models;

namespace Hostel2._0.Models.ViewModels.DashboardViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalHostels { get; set; }
        public int TotalUsers { get; set; }
        public int PendingApprovals { get; set; }
        public List<Payment> RecentPayments { get; set; } = new List<Payment>();
    }

    public class ManagerDashboardViewModel
    {
        public required Hostel Hostel { get; set; }
        public int TotalRooms { get; set; }
        public int TotalStudents { get; set; }
        public int AvailableRooms { get; set; }
        public decimal PendingPayments { get; set; }
        public int ActiveNotices { get; set; }
        public List<Notice> RecentNotices { get; set; } = new List<Notice>();
        public List<ActivityViewModel> RecentActivities { get; set; } = new List<ActivityViewModel>();

        // Added for richer dashboard
        public List<Room> Rooms { get; set; } = new List<Room>();
        public List<ApplicationUser> Students { get; set; } = new List<ApplicationUser>();
        public List<Payment> RecentPayments { get; set; } = new List<Payment>();
        public List<Hostel2._0.Models.MealManagement.Meal> Meals { get; set; } = new List<Hostel2._0.Models.MealManagement.Meal>();
        public List<Hostel2._0.Models.MealManagement.MealPlan> MealPlans { get; set; } = new List<Hostel2._0.Models.MealManagement.MealPlan>();
    }

    public class ActivityViewModel
    {
        public DateTime Timestamp { get; set; }
        public string ActivityType { get; set; }
        public string UserName { get; set; }
        public string Details { get; set; }
    }

    public class StudentDashboardViewModel
    {
        public required Hostel Hostel { get; set; }
        public Room? Room { get; set; }
        public string RoomNumber { get; set; }
        public decimal CurrentBalance { get; set; }
        public List<Notice> RecentNotices { get; set; } = new List<Notice>();
        public List<Payment> RecentPayments { get; set; } = new List<Payment>();
        public List<MealManagement.StudentMeal> RecentMeals { get; set; } = new List<MealManagement.StudentMeal>();
    }
} 