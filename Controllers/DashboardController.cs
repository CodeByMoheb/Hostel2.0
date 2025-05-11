using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models;
using Hostel2._0.Models.ViewModels;
using Hostel2._0.Models.ViewModels.DashboardViewModels;
using Hostel2._0.Models.Enums;
using Hostel2._0.Models.MealManagement;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Hostel2._0.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (User.IsInRole("Student"))
            {
                return await StudentDashboard();
            }
            else if (User.IsInRole("HostelManager"))
            {
                return RedirectToAction("ManagerDashboard");
            }
            else if (User.IsInRole("Admin"))
            {
                return RedirectToAction("AdminDashboard");
            }

            return NotFound("Role not found");
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentDashboard()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var student = await _context.Students
                    .Include(s => s.Room)
                    .Include(s => s.User)
                    .Include(s => s.Hostel)
                    .Include(s => s.Payments)
                    .Include(s => s.MaintenanceRequests)
                    .Include(s => s.StudentMeals)
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    return RedirectToAction("Join", "Hostel");
                }

                var viewModel = new Hostel2._0.Models.ViewModels.StudentDashboardViewModel
                {
                    Student = student,
                    Room = student.Room,
                    RecentPayments = await _context.Payments
                        .Where(p => p.StudentId == student.Id)
                        .OrderByDescending(p => p.PaymentDate)
                        .Take(5)
                        .ToListAsync(),
                    RecentMaintenanceRequests = await _context.MaintenanceRequests
                        .Where(m => m.StudentId == student.Id)
                        .OrderByDescending(m => m.CreatedAt)
                        .Take(5)
                        .ToListAsync(),
                    UpcomingMeals = await _context.StudentMeals
                        .Include(m => m.Meal)
                        .Where(m => m.StudentId == student.Id && m.MealDate >= DateTime.Today)
                        .OrderBy(m => m.MealDate)
                        .Take(5)
                        .ToListAsync(),
                    RecentNotices = await _context.Notices
                        .Where(n => n.HostelId == student.HostelId && n.IsActive)
                        .OrderByDescending(n => n.CreatedAt)
                        .Take(5)
                        .ToListAsync(),
                    TotalPaidAmount = await _context.Payments
                        .Where(p => p.StudentId == student.Id && p.Status == PaymentStatus.Completed)
                        .SumAsync(p => p.Amount),
                    PendingPaymentAmount = await _context.Payments
                        .Where(p => p.StudentId == student.Id && p.Status == PaymentStatus.Pending)
                        .SumAsync(p => p.Amount),
                    TotalMaintenanceRequests = await _context.MaintenanceRequests
                        .CountAsync(m => m.StudentId == student.Id),
                    PendingMaintenanceRequests = await _context.MaintenanceRequests
                        .CountAsync(m => m.StudentId == student.Id && m.Status == MaintenanceStatus.Pending),
                    JoinCode = student.Hostel?.JoinCode,
                    IsApproved = student.IsApproved,
                    HasRoom = student.Room != null,
                    DaysInHostel = (int)(DateTime.Now - student.CreatedAt).TotalDays,
                    MealsConsumed = await _context.StudentMeals
                        .CountAsync(m => m.StudentId == student.Id && m.IsConsumed),
                    DocumentsSubmitted = await _context.Documents
                        .CountAsync(d => d.StudentId == student.Id),
                    AttendancePercentage = await CalculateAttendancePercentage(student.Id)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error loading student dashboard: {ex.Message}");
                
                // Return a simple error view
                ViewBag.ErrorMessage = "An error occurred loading your dashboard. Please try again later.";
                return View("Error");
            }
        }

        private async Task<double> CalculateAttendancePercentage(int studentId)
        {
            var totalMeals = await _context.StudentMeals
                .CountAsync(m => m.StudentId == studentId && m.MealDate <= DateTime.Today);

            if (totalMeals == 0) return 100;

            var attendedMeals = await _context.StudentMeals
                .CountAsync(m => m.StudentId == studentId && m.IsConsumed && m.MealDate <= DateTime.Today);

            return Math.Round((double)attendedMeals / totalMeals * 100, 2);
        }

        [Authorize(Roles = "HostelManager")]
        public async Task<IActionResult> ManagerDashboard()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels
                .FirstOrDefaultAsync(h => h.ManagerId == userId);

            var viewModel = new ManagerDashboardViewModel();

            if (hostel != null)
            {
                viewModel.Hostel = hostel;
                viewModel.TotalRooms = await _context.Rooms.CountAsync(r => r.HostelId == hostel.Id);
                viewModel.OccupiedRooms = await _context.Rooms.CountAsync(r => r.HostelId == hostel.Id && r.CurrentOccupancy > 0);
                viewModel.TotalStudents = await _context.Students.CountAsync(s => s.HostelId == hostel.Id);
                viewModel.AvailableRooms = viewModel.TotalRooms - viewModel.OccupiedRooms;
                viewModel.PendingPayments = await _context.Payments.CountAsync(p => p.HostelId == hostel.Id && p.Status == PaymentStatus.Pending);
                viewModel.ActiveNotices = await _context.Notices.CountAsync(n => n.HostelId == hostel.Id && n.IsActive);

                // Load related data
                viewModel.Rooms = await _context.Rooms
                    .Where(r => r.HostelId == hostel.Id)
                    .OrderBy(r => r.RoomNumber)
                    .ToListAsync();

                viewModel.Students = await _context.Students
                    .Include(s => s.Room)
                    .Where(s => s.HostelId == hostel.Id)
                    .OrderBy(s => s.Name)
                    .ToListAsync();

                viewModel.RecentNotices = await _context.Notices
                    .Where(n => n.HostelId == hostel.Id)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                viewModel.RecentPayments = await _context.Payments
                    .Include(p => p.Student)
                    .ThenInclude(s => s.User)
                    .Where(p => p.HostelId == hostel.Id)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .ToListAsync();

                viewModel.Meals = await _context.Meals
                    .Where(m => m.HostelId == hostel.Id && m.IsActive)
                    .ToListAsync();

                viewModel.MealPlans = await _context.MealPlans
                    .Where(m => m.HostelId == hostel.Id)
                    .ToListAsync();
            }

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalHostels = await _context.Hostels.CountAsync(),
                ActiveHostels = await _context.Hostels.CountAsync(h => h.IsActive),
                PendingApprovals = await _context.Hostels.CountAsync(h => !h.IsActive),
                RecentSubscriptions = await _context.HostelSubscriptions
                    .Include(hs => hs.Hostel)
                    .OrderByDescending(hs => hs.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
} 