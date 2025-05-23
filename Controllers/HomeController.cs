using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Hostel2._0.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Hostel2._0.Data;

namespace Hostel2._0.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            bool isStudentWithoutHostel = false;
            if (User.Identity.IsAuthenticated && User.IsInRole("Student"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var hasStudentRecord = _context.Students.Any(s => s.UserId == user.Id);
                    var hasHostelId = user.HostelId.HasValue && user.HostelId.Value > 0;
                    isStudentWithoutHostel = !hasStudentRecord && !hasHostelId;
                }
            }
            ViewBag.IsStudentWithoutHostel = isStudentWithoutHostel;
            return View();
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier ?? "No Request ID" });
        }
    }
} 