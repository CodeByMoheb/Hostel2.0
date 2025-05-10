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
            var model = new HostelCreateViewModel();
            return View(model);
        }

        // POST: Hostel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "HostelManager")]
        public async Task<IActionResult> Create(HostelCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) return NotFound("User not found");

                // Check if user already has a hostel
                var existingHostel = await _context.Hostels
                    .FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
                    
                if (existingHostel != null)
                {
                    ModelState.AddModelError(string.Empty, "You already have a registered hostel.");
                    return View(model);
                }

                // Generate a unique registration code
                string registrationCode = GenerateUniqueCode();
                
                // Create the hostel
                var hostel = new Hostel
                {
                    Name = model.Name,
                    Address = model.Address,
                    City = model.City,
                    State = model.State,
                    PostalCode = model.PostalCode,
                    ContactNumber = model.ContactNumber,
                    Email = model.Email,
                    ManagerId = currentUser.Id,
                    RegistrationCode = registrationCode,
                    IsActive = false, // Will be activated after approval
                    SubscriptionStartDate = DateTime.Now,
                    SubscriptionEndDate = DateTime.Now.AddDays(30), // 30-day trial
                    MonthlyFee = 0 // Will be set when subscription is chosen
                };
                
                _context.Add(hostel);
                await _context.SaveChangesAsync();
                
                // Create an approval request
                var approval = new HostelApproval
                {
                    HostelId = hostel.Id,
                    Status = ApprovalStatus.Pending
                };
                
                _context.Add(approval);
                await _context.SaveChangesAsync();
                
                return RedirectToAction("PendingApproval");
            }
            
            return View(model);
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
                .FirstOrDefaultAsync(a => a.HostelId == hostel.Id);
                
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
            var pendingApprovals = await _context.HostelApprovals
                .Include(a => a.Hostel)
                .ThenInclude(h => h.Manager)
                .Where(a => a.Status == ApprovalStatus.Pending)
                .ToListAsync();
                
            return View(pendingApprovals);
        }

        // POST: Hostel/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(string id, string comments)
        {
            var approval = await _context.HostelApprovals.FindAsync(id);
            if (approval == null)
            {
                return NotFound();
            }
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound("User not found");
            
            approval.Status = ApprovalStatus.Approved;
            approval.AdminId = currentUser.Id;
            approval.ApprovedAt = DateTime.Now;
            approval.Comments = comments;
            approval.UpdatedAt = DateTime.Now;
            
            // Activate the hostel
            var hostel = await _context.Hostels.FindAsync(approval.HostelId);
            if (hostel != null)
            {
                hostel.IsActive = true;
                hostel.UpdatedAt = DateTime.Now;
                _context.Update(hostel);
            }
            
            _context.Update(approval);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("PendingApprovals");
        }

        // POST: Hostel/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(string id, string comments)
        {
            var approval = await _context.HostelApprovals.FindAsync(id);
            if (approval == null)
            {
                return NotFound();
            }
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound("User not found");
            
            approval.Status = ApprovalStatus.Rejected;
            approval.AdminId = currentUser.Id;
            approval.ApprovedAt = DateTime.Now;
            approval.Comments = comments;
            approval.UpdatedAt = DateTime.Now;
            
            _context.Update(approval);
            await _context.SaveChangesAsync();
            
            return RedirectToAction("PendingApprovals");
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
        public async Task<IActionResult> Subscribe(string hostelId, string planId, SubscriptionBillingCycle billingCycle)
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
            
            if (billingCycle == SubscriptionBillingCycle.Monthly)
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
                Status = SubscriptionStatus.Active
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
                .Where(hs => hs.HostelId == hostel.Id && hs.Status == SubscriptionStatus.Active)
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
        public IActionResult JoinHostel()
        {
            var model = new JoinHostelViewModel();
            return View(model);
        }

        // POST: Hostel/JoinHostel
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> JoinHostel(JoinHostelViewModel model)
        {
            if (ModelState.IsValid)
            {
                var hostel = await _context.Hostels
                    .FirstOrDefaultAsync(h => h.RegistrationCode == model.RegistrationCode && h.IsActive);
                    
                if (hostel == null)
                {
                    ModelState.AddModelError("RegistrationCode", "Invalid registration code or the hostel is not active.");
                    return View(model);
                }
                
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) return NotFound("User not found");
                
                // Check if user is already in a hostel
                if (!string.IsNullOrEmpty(currentUser.HostelId))
                {
                    ModelState.AddModelError(string.Empty, "You are already registered in a hostel.");
                    return View(model);
                }
                
                // Update user's hostel
                currentUser.HostelId = hostel.Id;
                await _userManager.UpdateAsync(currentUser);
                
                return RedirectToAction("StudentDashboard", "Dashboard");
            }
            
            return View(model);
        }

        private string GenerateUniqueCode()
        {
            // Generate a 8-character alphanumeric code
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
                
            // Check if code already exists, regenerate if needed
            while (_context.Hostels.Any(h => h.RegistrationCode == code))
            {
                code = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            
            return code;
        }
    }
} 