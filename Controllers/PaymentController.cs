using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models;
using System.Threading.Tasks;
using System.Linq;
using System;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Controllers
{
    [Authorize(Roles = "Manager")]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null)
            {
                return NotFound("Hostel not found");
            }

            var payments = await _context.Payments
                .Include(p => p.Student)
                .Where(p => p.HostelId == hostel.Id)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(payments);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Payment payment)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
                
                if (hostel == null)
                {
                    return NotFound("Hostel not found");
                }

                payment.HostelId = hostel.Id;
                payment.CreatedAt = DateTime.Now;
                payment.Status = PaymentStatus.Pending;
                _context.Add(payment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || payment.HostelId != hostel.Id)
            {
                return NotFound("Payment not found in your hostel");
            }

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Payment payment)
        {
            if (id != payment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
                    
                    if (hostel == null || payment.HostelId != hostel.Id)
                    {
                        return NotFound("Payment not found in your hostel");
                    }

                    _context.Update(payment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentExists(payment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(payment);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Student)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || payment.HostelId != hostel.Id)
            {
                return NotFound("Payment not found in your hostel");
            }

            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, PaymentStatus status)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || payment.HostelId != hostel.Id)
            {
                return NotFound("Payment not found in your hostel");
            }

            payment.Status = status;
            if (status == PaymentStatus.Completed)
            {
                payment.PaymentDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = payment.Id });
        }

        public async Task<IActionResult> GenerateReceipt(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Student)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || payment.HostelId != hostel.Id)
            {
                return NotFound("Payment not found in your hostel");
            }

            return View(payment);
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }
    }
} 