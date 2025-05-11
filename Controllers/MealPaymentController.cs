using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models.MealManagement;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Hostel2._0.Controllers
{
    [Authorize(Roles = "Student")]
    public class MealPaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MealPaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
            {
                return NotFound("Student profile not found.");
            }
            var payments = await _context.MealPayments
                .Include(mp => mp.StudentMeal)
                .Where(mp => mp.StudentMeal.StudentId == student.Id)
                .OrderByDescending(mp => mp.PaymentDate)
                .ToListAsync();
            return View(payments);
        }
    }
} 