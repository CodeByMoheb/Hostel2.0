using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Hostel2._0.Data;
using Hostel2._0.Models;
using Hostel2._0.Models.Enums;
using Hostel2._0.Models.ViewModels;
using Hostel2._0.Models.ViewModels.DashboardViewModels;

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
                return NotFound();

            switch (user.Role)
            {
                case UserRole.Admin:
                    return RedirectToAction("AdminDashboard");
                case UserRole.HostelManager:
                    return RedirectToAction("ManagerDashboard");
                case UserRole.Student:
                    return RedirectToAction("StudentDashboard");
                default:
                    return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalHostels = await _context.Hostels.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                PendingApprovals = await _context.Users
                    .Where(u => u.Role == UserRole.HostelManager && !u.IsActive)
                    .CountAsync(),
                RecentPayments = await _context.Payments
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Include(p => p.User)
                    .Include(p => p.Hostel)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "HostelManager")]
        public async Task<IActionResult> ManagerDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var userWithHostel = await _context.Users
                .Include(u => u.Hostel)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userWithHostel?.Hostel == null)
                return NotFound();

            var hostelId = userWithHostel.HostelId;

            var viewModel = new ManagerDashboardViewModel
            {
                Hostel = userWithHostel.Hostel,
                TotalRooms = await _context.Rooms.Where(r => r.HostelId == hostelId).CountAsync(),
                TotalStudents = await _context.Users.Where(u => u.HostelId == hostelId && u.Role == UserRole.Student).CountAsync(),
                RecentNotices = await _context.Notices.Where(n => n.HostelId == hostelId).OrderByDescending(n => n.CreatedAt).Take(5).ToListAsync(),
                Rooms = await _context.Rooms.Where(r => r.HostelId == hostelId).ToListAsync(),
                Students = await _context.Users.Where(u => u.HostelId == hostelId && u.Role == UserRole.Student).ToListAsync(),
                RecentPayments = await _context.Payments.Where(p => p.HostelId == hostelId).OrderByDescending(p => p.CreatedAt).Take(5).ToListAsync(),
                Meals = await _context.Meals.Where(m => m.HostelId == hostelId).ToListAsync(),
                MealPlans = await _context.MealPlans.Where(mp => mp.HostelId == hostelId).OrderByDescending(mp => mp.CreatedAt).Take(3).ToListAsync()
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentDashboard()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var userWithDetails = await _context.Users
                .Include(u => u.Hostel)
                .Include(u => u.Room)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (userWithDetails?.Hostel == null)
                return NotFound();

            var viewModel = new StudentDashboardViewModel
            {
                Hostel = userWithDetails.Hostel,
                Room = userWithDetails.Room,
                RecentNotices = await _context.Notices
                    .Where(n => n.HostelId == userWithDetails.HostelId)
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(5)
                    .ToListAsync(),
                RecentPayments = await _context.Payments
                    .Where(p => p.UserId == userWithDetails.Id)
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }
    }
} 