using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models;
using Hostel2._0.Models.Enums;
using Hostel2._0.Models.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json;

namespace Hostel2._0.Controllers
{
    [Authorize]
    public class HostelController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HostelController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Hostel
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var hostels = await _context.Hostels
                .Include(h => h.Manager)
                .ToListAsync();
            
            return View(hostels);
        }

        // GET: Hostel/Create
        [Authorize(Roles = "HostelManager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Hostel/Create
        [HttpPost]
        [Authorize(Roles = "HostelManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var hostel = new Hostel
            {
                Name = name,
                Address = "Pending Approval", // default
                City = "Pending",
                State = "Pending",
                PostalCode = "00000",
                Country = "Pending",
                PhoneNumber = "0000000000",
                Email = user.Email ?? "pending@hostel.com",
                ManagerId = user.Id,
                JoinCode = GenerateJoinCode(),
                IsActive = false, // Pending admin approval
                CreatedAt = DateTime.UtcNow
            };
            _context.Hostels.Add(hostel);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Hostel created and pending admin approval.";
            return RedirectToAction("PendingApproval");
        }

        // GET: Hostel/PendingApproval
        [Authorize(Roles = "HostelManager")]
        public async Task<IActionResult> PendingApproval()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == user.Id);
            if (hostel == null) return RedirectToAction("Create");

            var model = new Hostel2._0.Models.ViewModels.HostelApprovalViewModel
            {
                Hostel = hostel,
                ApprovalStatus = hostel.IsActive
                    ? Hostel2._0.Models.Enums.ApprovalStatus.Approved
                    : Hostel2._0.Models.Enums.ApprovalStatus.Pending,
                Comments = null // You can fetch comments from HostelApproval if you use it
            };

            return View(model);
        }

        // GET: Hostel/PendingApprovals
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PendingApprovals()
        {
            var pendingHostels = await _context.Hostels
                .Include(h => h.Manager)
                .Where(h => !h.IsActive)
                .ToListAsync();

            return View(pendingHostels);
        }

        // POST: Hostel/Approve/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveHostel(int id)
        {
            var hostel = await _context.Hostels.FindAsync(id);
            if (hostel == null)
            {
                return NotFound();
            }

            hostel.IsActive = true;
            await _context.SaveChangesAsync();

            // TODO: Send notification to hostel manager

            return RedirectToAction(nameof(PendingApprovals));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectHostel(int id, string rejectionReason)
        {
            var hostel = await _context.Hostels.FindAsync(id);
            if (hostel == null)
            {
                return NotFound();
            }

            // Create rejection record
            var rejection = new HostelApproval
            {
                HostelId = hostel.Id,
                Status = ApprovalStatus.Rejected,
                AdminId = _userManager.GetUserId(User),
                ApprovedAt = DateTime.UtcNow,
                Comments = rejectionReason ?? "Hostel registration rejected by admin."
            };

            // Mark the hostel as rejected
            hostel.IsActive = false;
            hostel.Status = "Rejected";
            hostel.RejectionReason = rejectionReason;

            _context.HostelApprovals.Add(rejection);
            await _context.SaveChangesAsync();

            // TODO: Send notification to hostel manager

            return RedirectToAction(nameof(PendingApprovals));
        }

        // GET: Hostel/Reapply
        [Authorize(Roles = "HostelManager")]
        public async Task<IActionResult> Reapply()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == user.Id);
            if (hostel == null) return NotFound();

            if (hostel.Status != "Rejected")
            {
                return RedirectToAction("PendingApproval");
            }

            return View(hostel);
        }

        // POST: Hostel/Reapply
        [HttpPost]
        [Authorize(Roles = "HostelManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reapply(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var hostel = await _context.Hostels.FindAsync(id);
            if (hostel == null) return NotFound();

            // Verify that the hostel belongs to the current user
            if (hostel.ManagerId != user.Id)
            {
                return Unauthorized();
            }

            // Reset the hostel status
            hostel.Status = "Pending";
            hostel.IsActive = false;
            hostel.RejectionReason = null;

            // Update existing approval record or create new one if none exists
            var existingApproval = await _context.HostelApprovals
                .FirstOrDefaultAsync(a => a.HostelId == hostel.Id);

            if (existingApproval != null)
            {
                // Update existing approval record
                existingApproval.Status = ApprovalStatus.Pending;
                existingApproval.AdminId = null;
                existingApproval.ApprovedAt = null;
                existingApproval.Comments = "Hostel reapplication submitted";
            }
            else
            {
                // Create new approval record if none exists
                var approval = new HostelApproval
                {
                    HostelId = hostel.Id,
                    Status = ApprovalStatus.Pending,
                    AdminId = null,
                    ApprovedAt = null,
                    Comments = "Hostel reapplication submitted"
                };
                _context.HostelApprovals.Add(approval);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your hostel application has been resubmitted for review.";
                return RedirectToAction("PendingApproval");
        }

        // GET: Hostel/JoinHostel
        [Authorize(Roles = "Student")]
        public IActionResult Join()
        {
            return View();
        }

        // POST: Hostel/JoinHostel
        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(string joinCode)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            if (string.IsNullOrWhiteSpace(joinCode))
            {
                ModelState.AddModelError("", "Join code cannot be empty.");
                return View();
            }

            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.JoinCode == joinCode && h.IsActive);
            if (hostel == null)
            {
                ModelState.AddModelError("", "Invalid or inactive join code.");
                return View();
            }

            // Check if student is already part of this hostel or any other hostel
             var existingStudentAssociation = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (existingStudentAssociation != null)
            {
                if (existingStudentAssociation.HostelId == hostel.Id)
                {
                     ModelState.AddModelError("", "You are already a member of this hostel.");
                }
                else
                {
                     ModelState.AddModelError("", "You are already a member of another hostel. You can only be part of one hostel at a time.");
                }
                return View();
            }

            // Create a new Student record
            var student = new Student
            {
                UserId = user.Id,
                HostelId = hostel.Id,
                Name = user.Name, // Assuming Name is populated in ApplicationUser
                Email = user.Email ?? "", // Ensure Email is not null
                PhoneNumber = user.PhoneNumber ?? "", // Ensure PhoneNumber is not null
                IsActive = false, // Student becomes active after manager approval or auto-approval
                CreatedAt = DateTime.UtcNow,
                StudentId = $"STD-{DateTime.UtcNow.Ticks % 100000}" // Example Student ID
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Successfully joined {hostel.Name}! Your application is pending manager approval.";
            return RedirectToAction("JoinWithCode", "Hostel", new { hostelId = hostel.Id });
        }

        // GET: Hostel/JoinWithCode
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> JoinWithCode(int hostelId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var existingStudentAssociation = await _context.Students
                .FirstOrDefaultAsync(s => s.UserId == user.Id);

            if (existingStudentAssociation != null)
            {
            return RedirectToAction("StudentDashboard", "Dashboard");
            }

            var hostel = await _context.Hostels.FindAsync(hostelId);
            if (hostel == null)
            {
                return NotFound();
            }

            var viewModel = new JoinHostelViewModel
            {
                RegistrationCode = hostel.JoinCode
            };

            return View(viewModel);
        }

        private string GenerateJoinCode()
        {
            // Generate a random 8-character alphanumeric code
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return code;
        }
    }
} 