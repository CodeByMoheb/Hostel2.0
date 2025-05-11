using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models;
using Hostel2._0.Models.Enums;
using System.Threading.Tasks;
using System.Linq;

namespace Hostel2._0.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SubscriptionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public SubscriptionController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Subscription/Plans
        public async Task<IActionResult> Plans()
        {
            var plans = await _context.SubscriptionPlans.ToListAsync();
            return View(plans);
        }

        // GET: Subscription/CreatePlan
        public IActionResult CreatePlan()
        {
            return View();
        }

        // POST: Subscription/CreatePlan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePlan(SubscriptionPlan model)
        {
            if (ModelState.IsValid)
            {
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Plans));
            }
            return View(model);
        }

        // GET: Subscription/EditPlan/5
        public async Task<IActionResult> EditPlan(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // POST: Subscription/EditPlan/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPlan(int id, SubscriptionPlan model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Plans));
            }
            return View(model);
        }

        // GET: Subscription/DeletePlan/5
        public async Task<IActionResult> DeletePlan(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var plan = await _context.SubscriptionPlans
                .FirstOrDefaultAsync(m => m.Id == id);
            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // POST: Subscription/DeletePlan/5
        [HttpPost, ActionName("DeletePlan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePlanConfirmed(int id)
        {
            var plan = await _context.SubscriptionPlans.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }

            // Check if any hostels are using this plan
            var inUse = await _context.HostelSubscriptions
                .AnyAsync(hs => hs.SubscriptionPlanId == id && hs.Status == Models.Enums.SubscriptionStatus.Active);

            if (inUse)
            {
                ModelState.AddModelError(string.Empty, "This plan cannot be deleted because it is being used by active hostels.");
                return View(plan);
            }

            _context.SubscriptionPlans.Remove(plan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Plans));
        }

        // GET: Subscription/HostelSubscriptions
        public async Task<IActionResult> HostelSubscriptions()
        {
            var subscriptions = await _context.HostelSubscriptions
                .Include(hs => hs.Hostel)
                .Include(hs => hs.SubscriptionPlan)
                .OrderByDescending(hs => hs.StartDate)
                .ToListAsync();

            return View(subscriptions);
        }

        private bool PlanExists(int id)
        {
            return _context.SubscriptionPlans.Any(e => e.Id == id);
        }
    }
} 