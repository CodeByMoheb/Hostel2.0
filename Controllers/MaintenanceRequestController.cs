using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models;
using Hostel2._0.Models.Enums;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Security.Claims;

namespace Hostel2._0.Controllers
{
    [Authorize]
    public class MaintenanceRequestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MaintenanceRequestController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return RedirectToAction("JoinHostel", "Student");
            }

            var requests = await _context.MaintenanceRequests
                .Include(m => m.Student)
                .Where(m => m.StudentId == student.Id)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();

            return View(requests);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MaintenanceRequest request)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.UserId == userId);

                if (student == null)
                {
                    return RedirectToAction("JoinHostel", "Student");
                }

                request.StudentId = student.Id;
                request.Status = Models.Enums.MaintenanceStatus.Pending;
                request.CreatedAt = System.DateTime.Now;

                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        [HttpGet]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (student == null)
            {
                return RedirectToAction("JoinHostel", "Student");
            }

            var request = await _context.MaintenanceRequests
                .Include(m => m.Student)
                .FirstOrDefaultAsync(m => m.Id == id && m.StudentId == student.Id);

            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, MaintenanceStatus status)
        {
            var request = await _context.MaintenanceRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || request.HostelId != hostel.Id)
            {
                return NotFound("Request not found in your hostel");
            }

            request.Status = status;
            if (status == MaintenanceStatus.Completed)
            {
                request.CompletedDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = request.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignStaff(int id, string staffId)
        {
            var request = await _context.MaintenanceRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || request.HostelId != hostel.Id)
            {
                return NotFound("Request not found in your hostel");
            }

            request.AssignedTo = staffId;
            request.Status = MaintenanceStatus.InProgress;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = request.Id });
        }

        public async Task<IActionResult> AddComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var request = await _context.MaintenanceRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || request.HostelId != hostel.Id)
            {
                return NotFound("Request not found in your hostel");
            }

            return View(request);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, string comment)
        {
            var request = await _context.MaintenanceRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || request.HostelId != hostel.Id)
            {
                return NotFound("Request not found in your hostel");
            }

            request.ResolutionNotes = string.IsNullOrEmpty(request.ResolutionNotes) 
                ? comment 
                : request.ResolutionNotes + "\n" + comment;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = request.Id });
        }

        private bool MaintenanceRequestExists(int id)
        {
            return _context.MaintenanceRequests.Any(e => e.Id == id);
        }
    }
} 