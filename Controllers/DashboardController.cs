using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models;
using Hostel2._0.Models.Enums;
using Hostel2._0.Models.ViewModels;
using Hostel2._0.Models.ViewModels.DashboardViewModels;
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
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(nameof(AdminDashboard));
            }
            else if (User.IsInRole("HostelManager"))
            {
                return RedirectToAction(nameof(ManagerDashboard));
            }
            else if (User.IsInRole("Student"))
            {
                return RedirectToAction(nameof(StudentDashboard));
            }

            // If user has no role, redirect to login
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalHostels = await _context.Hostels.CountAsync(),
                TotalStudents = await _context.Students.CountAsync(),
                ActiveHostels = await _context.Hostels.CountAsync(h => h.IsActive),
                PendingApprovals = await _context.Hostels.CountAsync(h => !h.IsActive),
                TotalManagers = await _userManager.GetUsersInRoleAsync("HostelManager") is var managers ? managers.Count : 0,
                RecentHostels = await _context.Hostels
                    .OrderByDescending(h => h.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                RecentManagers = (await _userManager.GetUsersInRoleAsync("HostelManager"))
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .ToList()
            };

            return View(viewModel);
        }

        private async Task<int> CalculateOccupancyRate()
        {
            var totalRooms = await _context.Rooms.CountAsync();
            if (totalRooms == 0) return 0;

            var occupiedRooms = await _context.Rooms.CountAsync(r => r.Students.Any());
            return (int)((double)occupiedRooms / totalRooms * 100);
        }

        [Authorize(Roles = "HostelManager")]
        public async Task<IActionResult> ManagerDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var hostel = await _context.Hostels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.ManagerId == user.Id);

            if (hostel == null)
            {
                return RedirectToAction("Create", "Hostel");
            }

            if (!hostel.IsActive)
            {
                return RedirectToAction("PendingApproval", "Hostel");
            }

            var now = DateTime.Now;
            var payments = await _context.Payments.Where(p => p.HostelId == hostel.Id).ToListAsync();
            var totalPaymentsThisMonth = payments.Where(p => p.Status == PaymentStatus.Completed && p.PaymentDate.Month == now.Month && p.PaymentDate.Year == now.Year).Sum(p => p.Amount);
            var totalPendingPayments = payments.Where(p => p.Status == PaymentStatus.Pending).Sum(p => p.Amount);
            var studentsWithDues = await _context.Students.CountAsync(s => s.HostelId == hostel.Id && !_context.Payments.Any(p => p.StudentId == s.Id && p.Status == PaymentStatus.Completed && p.PaymentDate.Month == now.Month && p.PaymentDate.Year == now.Year));

            var viewModel = new ManagerDashboardViewModel
            {
                Hostel = hostel,
                TotalRooms = hostel.Rooms.Count,
                OccupiedRooms = hostel.Rooms.Count(r => r.Students.Any()),
                AvailableRooms = hostel.Rooms.Count(r => !r.Students.Any()),
                TotalStudents = await _context.Students.CountAsync(s => s.HostelId == hostel.Id && s.IsActive),
                ActiveNotices = await _context.Notices.CountAsync(n => n.HostelId == hostel.Id && n.IsActive),
                PendingPayments = await _context.Payments
                    .Where(p => p.HostelId == hostel.Id && p.Status == PaymentStatus.Pending)
                    .SumAsync(p => p.Amount),
                TotalPaymentsThisMonth = totalPaymentsThisMonth,
                TotalPendingPayments = totalPendingPayments,
                StudentsWithDues = studentsWithDues,
                Rooms = await _context.Rooms
                    .Where(r => r.HostelId == hostel.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                Students = await _context.Students
                    .Include(s => s.Room)
                    .Where(s => s.HostelId == hostel.Id)
                    .OrderByDescending(s => s.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                RecentNotices = await _context.Notices
                    .Where(n => n.HostelId == hostel.Id)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                RecentPayments = await _context.Payments
                    .Include(p => p.Student)
                    .Where(p => p.HostelId == hostel.Id)
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(5)
                    .ToListAsync(),
                RecentMaintenanceRequests = await _context.MaintenanceRequests
                    .Where(m => m.HostelId == hostel.Id)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (student == null)
            {
                return NotFound("Student record not found.");
            }

            var totalPaid = await _context.Payments
                .Where(p => p.StudentId == student.Id && p.Status == PaymentStatus.Completed)
                .SumAsync(p => p.Amount);
            var pendingPayment = await _context.Payments
                .Where(p => p.StudentId == student.Id && p.Status == PaymentStatus.Pending)
                .SumAsync(p => p.Amount);
            var pendingRequests = await _context.MaintenanceRequests
                .CountAsync(r => r.StudentId == student.Id && r.Status == MaintenanceStatus.Pending);
            // AttendancePercentage is not implemented, set to 0
            var recentNotices = await _context.Notices
                .Where(n => n.HostelId == student.HostelId && n.IsActive)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            var viewModel = new StudentDashboardViewModel
            {
                Student = student,
                Room = student.Room,
                IsApproved = student.IsApproved,
                HasRoom = student.Room != null,
                TotalPaidAmount = totalPaid,
                PendingPaymentAmount = pendingPayment,
                AttendancePercentage = 0,
                PendingMaintenanceRequests = pendingRequests,
                RecentNotices = recentNotices,
                RecentPayments = await _context.Payments
                    .Where(p => p.StudentId == student.Id)
                    .OrderByDescending(p => p.PaymentDate)
                    .Take(5)
                    .ToListAsync(),
                RecentMaintenanceRequests = await _context.MaintenanceRequests
                    .Where(m => m.StudentId == student.Id)
                    .OrderByDescending(m => m.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
} 