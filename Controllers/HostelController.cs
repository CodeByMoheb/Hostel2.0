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
            return View(new HostelCreateViewModel());
        }

        // POST: Hostel/Create
        [HttpPost]
        [Authorize(Roles = "HostelManager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HostelCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                var hostel = new Hostel
                {
                    Name = viewModel.Name,
                    Address = viewModel.Address,
                    City = viewModel.City,
                    State = viewModel.State,
                    PostalCode = viewModel.PostalCode,
                    ContactNumber = viewModel.ContactNumber,
                    Email = viewModel.Email,
                    ManagerId = userId,
                    IsActive = false,
                    JoinCode = GenerateJoinCode(),
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = userId
                };
                
                _context.Add(hostel);
                await _context.SaveChangesAsync();

                // Create a HostelApproval record
                var hostelApproval = new HostelApproval
                {
                    HostelId = hostel.Id,
                    Status = ApprovalStatus.Pending,
                    CreatedAt = DateTime.UtcNow
                };
                
                _context.HostelApprovals.Add(hostelApproval);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Hostel created successfully. Waiting for admin approval.";
                return RedirectToAction("ManagerDashboard", "Dashboard");
            }
            return View(viewModel);
        }

        // GET: Hostel/PendingApproval
        [Authorize(Roles = "HostelManager")]
        public async Task<IActionResult> PendingApproval()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound("User not found");
            
            // Get manager's hostel that is pending approval
            var hostel = await _context.Hostels
                .Include(h => h.Manager)
                .FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
                
            if (hostel == null)
            {
                return RedirectToAction("Create");
            }
            
            var approval = await _context.HostelApprovals
                .FirstOrDefaultAsync(a => hostel != null && a.HostelId == hostel.Id);
                
            var viewModel = new HostelApprovalViewModel
            {
                Hostel = hostel,
                ApprovalStatus = approval?.Status ?? ApprovalStatus.Pending,
                Comments = approval?.Comments
            };
            
            return View(viewModel);
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

        // GET: Hostel/ChooseSubscription
        [Authorize(Roles = "HostelManager")]
        public async Task<IActionResult> ChooseSubscription()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound("User not found");
            
            // Get manager's active hostel
            var hostel = await _context.Hostels
                .FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id && h.IsActive);
                
            if (hostel == null)
            {
                return RedirectToAction("PendingApproval");
            }
            
            // Get all active subscription plans
            var plans = await _context.SubscriptionPlans
                .Where(p => p.IsActive)
                .ToListAsync();
                
            var viewModel = new ChooseSubscriptionViewModel
            {
                HostelId = hostel.Id,
                HostelName = hostel.Name,
                SubscriptionPlans = plans
            };
            
            return View(viewModel);
        }

        // POST: Hostel/Subscribe
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HostelManager")]
        public async Task<IActionResult> Subscribe(int hostelId, int planId, BillingCycle billingCycle)
        {
            var hostel = await _context.Hostels.FindAsync(hostelId);
            var plan = await _context.SubscriptionPlans.FindAsync(planId);
            
            if (hostel == null || plan == null)
            {
                return NotFound();
            }
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound("User not found");
            
            if (hostel.ManagerId != currentUser.Id)
            {
                return Forbid();
            }
            
            // Calculate subscription details
            DateTime startDate = DateTime.Now;
            DateTime endDate;
            decimal amount;
            
            if (billingCycle == BillingCycle.Monthly)
            {
                endDate = startDate.AddMonths(1);
                amount = plan.MonthlyPrice;
            }
            else // Yearly
            {
                endDate = startDate.AddYears(1);
                amount = plan.YearlyPrice;
            }
            
            // Create subscription
            var subscription = new HostelSubscription
            {
                HostelId = hostelId,
                SubscriptionPlanId = planId,
                StartDate = startDate,
                EndDate = endDate,
                Amount = amount,
                BillingCycle = billingCycle,
                Status = Models.Enums.SubscriptionStatus.Active
            };
            
            // Update hostel subscription details
            hostel.SubscriptionStartDate = startDate;
            hostel.SubscriptionEndDate = endDate;
            hostel.MonthlyFee = amount;
            hostel.UpdatedAt = DateTime.Now;
            
            _context.Add(subscription);
            _context.Update(hostel);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("SubscriptionDetails");
        }

        // GET: Hostel/SubscriptionDetails
        [Authorize(Roles = "HostelManager")]
        public async Task<IActionResult> SubscriptionDetails()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound("User not found");
            
            // Get manager's active hostel
            var hostel = await _context.Hostels
                .FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
                
            if (hostel == null)
            {
                return RedirectToAction("Create");
            }
            
            // Get active subscription
            var subscription = await _context.HostelSubscriptions
                .Include(hs => hs.SubscriptionPlan)
                .Where(hs => hs.HostelId == hostel.Id && hs.Status == Models.Enums.SubscriptionStatus.Active)
                .OrderByDescending(hs => hs.StartDate)
                .FirstOrDefaultAsync();
                
            var viewModel = new SubscriptionDetailsViewModel
            {
                Hostel = hostel,
                Subscription = subscription
            };
            
            return View(viewModel);
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
            var hostel = await _context.Hostels
                .FirstOrDefaultAsync(h => h.JoinCode == joinCode && h.IsActive);

            if (hostel == null)
            {
                ModelState.AddModelError("", "Invalid join code or hostel not active");
                return View();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = new Student
            {
                UserId = userId,
                HostelId = hostel.Id,
                IsApproved = false // Requires hostel manager approval
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Join request sent. Waiting for hostel manager approval.";
            return RedirectToAction("StudentDashboard", "Dashboard");
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