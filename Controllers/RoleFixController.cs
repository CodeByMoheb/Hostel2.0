using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Hostel2._0.Models;
using System.Threading.Tasks;

namespace Hostel2._0.Controllers
{
    public class RoleFixController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleFixController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [Route("fixroles")]
        public async Task<IActionResult> FixRoles()
        {
            // Admin
            var admin = await _userManager.FindByEmailAsync("mohebullah.cse@gmail.com");
            if (admin != null && !await _userManager.IsInRoleAsync(admin, "Admin"))
                await _userManager.AddToRoleAsync(admin, "Admin");

            // Manager
            var manager = await _userManager.FindByEmailAsync("manager@hostel.com");
            if (manager != null && !await _userManager.IsInRoleAsync(manager, "HostelManager"))
                await _userManager.AddToRoleAsync(manager, "HostelManager");

            // Student
            var student = await _userManager.FindByEmailAsync("student@hostel.com");
            if (student != null && !await _userManager.IsInRoleAsync(student, "Student"))
                await _userManager.AddToRoleAsync(student, "Student");

            return Content("Roles fixed! You can now access the dashboards. Remove this controller after use.");
        }

        [Route("fixmanagerflag")]
        public async Task<IActionResult> FixManagerFlag()
        {
            var manager = await _userManager.FindByEmailAsync("manager@hostel.com");
            if (manager != null && !manager.IsHostelManager)
            {
                manager.IsHostelManager = true;
                await _userManager.UpdateAsync(manager);
                return Content("Manager's IsHostelManager flag set to true. You should now be able to access the manager dashboard.");
            }
            return Content("Manager user not found or already has IsHostelManager = true.");
        }
    }
} 