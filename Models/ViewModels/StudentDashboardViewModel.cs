using System;
using System.Collections.Generic;
using Hostel2._0.Models;
using Hostel2._0.Models.MealManagement;

namespace Hostel2._0.Models.ViewModels
{
    public class StudentDashboardViewModel
    {
        public StudentDashboardViewModel()
        {
            RecentPayments = new List<Payment>();
            RecentMaintenanceRequests = new List<MaintenanceRequest>();
            UpcomingMeals = new List<StudentMeal>();
            RecentNotices = new List<Notice>();
        }

        public required Student Student { get; set; }
        public Room? Room { get; set; }
        public List<Payment> RecentPayments { get; set; }
        public List<MaintenanceRequest> RecentMaintenanceRequests { get; set; }
        public List<StudentMeal> UpcomingMeals { get; set; }
        public List<Notice> RecentNotices { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal PendingPaymentAmount { get; set; }
        public int TotalMaintenanceRequests { get; set; }
        public int PendingMaintenanceRequests { get; set; }
        public string? JoinCode { get; set; }
        public bool IsApproved { get; set; }
        public bool HasRoom { get; set; }
        
        // Statistics
        public int DaysInHostel { get; set; }
        public int MealsConsumed { get; set; }
        public int DocumentsSubmitted { get; set; }
        public double AttendancePercentage { get; set; }
    }
} 