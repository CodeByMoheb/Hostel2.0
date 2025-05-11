using System;
using System.Collections.Generic;
using Hostel2._0.Models;
using Hostel2._0.Models.MealManagement;

namespace Hostel2._0.Models.ViewModels.DashboardViewModels
{
    public class ManagerDashboardViewModel
    {
        public ManagerDashboardViewModel()
        {
            Rooms = new List<Room>();
            Students = new List<Student>();
            RecentNotices = new List<Notice>();
            RecentPayments = new List<Payment>();
            Meals = new List<Meal>();
            MealPlans = new List<MealPlan>();
            MaintenanceRequests = new List<MaintenanceRequest>();
            Documents = new List<Document>();
            RecentActivities = new List<Activity>();
            UpcomingMeals = new List<Meal>();
            ActiveMealPlans = new List<MealPlan>();
        }

        public Hostel? Hostel { get; set; }
        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int TotalStudents { get; set; }
        public int AvailableRooms { get; set; }
        public int PendingPayments { get; set; }
        public int ActiveNotices { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Student> Students { get; set; }
        public ICollection<Notice> RecentNotices { get; set; }
        public ICollection<Payment> RecentPayments { get; set; }
        public ICollection<Meal> Meals { get; set; }
        public ICollection<MealPlan> MealPlans { get; set; }
        public ICollection<MaintenanceRequest> MaintenanceRequests { get; set; }
        public ICollection<Document> Documents { get; set; }
        public ICollection<Activity> RecentActivities { get; set; }

        // Meal management
        public int TotalMeals { get; set; }
        public int UpcomingMealsCount { get; set; }
        public ICollection<Meal> UpcomingMeals { get; set; }
        public ICollection<MealPlan> ActiveMealPlans { get; set; }

        // Notice management
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
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
} 