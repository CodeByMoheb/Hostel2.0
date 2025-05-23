using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Hostel2._0.Models;
using Hostel2._0.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hostel2._0.Controllers
{
    public class DiagnosticController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public DiagnosticController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Route("diagnostic/userstatus")]
        public async Task<IActionResult> UserStatus()
        {
            var admin = await _userManager.FindByEmailAsync("mohebullah.cse@gmail.com");
            var manager = await _userManager.FindByEmailAsync("manager@hostel.com");
            var student = await _userManager.FindByEmailAsync("student@hostel.com");

            var adminRoles = admin != null ? await _userManager.GetRolesAsync(admin) : null;
            var managerRoles = manager != null ? await _userManager.GetRolesAsync(manager) : null;
            var studentRoles = student != null ? await _userManager.GetRolesAsync(student) : null;

            var managerHostel = manager != null ? _context.Hostels.FirstOrDefault(h => h.ManagerId == manager.Id) : null;
            var studentRecord = student != null ? _context.Students.FirstOrDefault(s => s.UserId == student.Id) : null;

            return Content($@"
Admin:
  Exists: {admin != null}
  Email: {admin?.Email}
  Roles: {string.Join(", ", adminRoles ?? new string[0])}

Manager:
  Exists: {manager != null}
  IsHostelManager: {manager?.IsHostelManager}
  Roles: {string.Join(", ", managerRoles ?? new string[0])}
  Has Hostel: {managerHostel != null}

Student:
  Exists: {student != null}
  Roles: {string.Join(", ", studentRoles ?? new string[0])}
  Has Student Record: {studentRecord != null}
");
        }
    }
} 