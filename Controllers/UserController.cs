using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models;
using Hostel2._0.Models.ViewModels;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UserController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: User - List all users
        public async Task<IActionResult> Index(string filter = "All")
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (filter != "All")
            {
                UserRole role;
                if (Enum.TryParse(filter, out role))
                {
                    usersQuery = usersQuery.Where(u => u.Role == role);
                }
            }

            var users = await usersQuery
                .Include(u => u.Hostel)
                .Include(u => u.Room)
                .ToListAsync();

            ViewBag.CurrentFilter = filter;
            return View(users);
        }

        // GET: User/Edit/id
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var hostels = await _context.Hostels.ToListAsync();
            
            var viewModel = new UserEditViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                HostelId = user.HostelId,
                Hostels = hostels
            };

            return View(viewModel);
        }

        // POST: User/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserEditViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null) return NotFound();

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.Role = model.Role ?? UserRole.Student;
                user.IsActive = model.IsActive;
                user.HostelId = model.HostelId;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    // Update role if changed
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    await _userManager.AddToRoleAsync(user, model.Role?.ToString() ?? "Student");

                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed; redisplay form
            model.Hostels = await _context.Hostels.ToListAsync();
            return View(model);
        }

        // GET: User/PendingApprovals
        public async Task<IActionResult> PendingApprovals()
        {
            var pendingUsers = await _userManager.Users
                .Where(u => !u.IsActive && u.Role != UserRole.Admin)
                .Include(u => u.Hostel)
                .ToListAsync();

            return View(pendingUsers);
        }

        // POST: User/Approve/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(PendingApprovals));
        }

        // POST: User/Reject/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(PendingApprovals));
        }

        // GET: User/Profile/id
        [AllowAnonymous]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var viewModel = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                HostelName = user.Hostel?.Name ?? string.Empty,
                RoomNumber = user.Room?.RoomNumber ?? string.Empty
            };

            return View(viewModel);
        }

        // POST: User/UpdateProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("Profile");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Profile", model);
        }
    }
} 