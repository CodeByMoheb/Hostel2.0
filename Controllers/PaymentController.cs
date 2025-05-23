using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models;
using System.Threading.Tasks;
using System.Linq;
using System;
using Hostel2._0.Models.Enums;
using Hostel2._0.Services;
using Hostel2._0.Models.ViewModels.PaymentViewModels;

namespace Hostel2._0.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PdfReceiptService _pdfReceiptService;

        public PaymentController(ApplicationDbContext context, PdfReceiptService pdfReceiptService)
        {
            _context = context;
            _pdfReceiptService = pdfReceiptService;
        }

        [Authorize(Roles = "Manager")]
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

        [Authorize(Roles = "Manager")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
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

        [Authorize(Roles = "Manager")]
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
        [Authorize(Roles = "Manager")]
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

        [Authorize(Roles = "Manager")]
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
        [Authorize(Roles = "Manager")]
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

        [Authorize(Roles = "Manager,Student")]
        public async Task<IActionResult> GenerateReceipt(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payment = await _context.Payments
                .Include(p => p.Student)
                .Include(p => p.Room)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isManager = User.IsInRole("Manager");
            var isStudent = User.IsInRole("Student");

            if (isManager)
            {
                var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
                if (hostel == null || payment.HostelId != hostel.Id)
                {
                    return NotFound("Payment not found in your hostel");
                }
            }
            else if (isStudent)
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
                if (student == null || payment.StudentId != student.Id)
                {
                    return NotFound("You are not authorized to view this receipt.");
                }
            }
            else
            {
                return Forbid();
            }

            try
            {
                var pdfBytes = _pdfReceiptService.GenerateReceipt(payment);
                return File(pdfBytes, "application/pdf", $"receipt_{payment.ReceiptNumber}.pdf");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to generate PDF receipt. Please contact support. Error: " + ex.Message;
                if (isStudent)
                    return RedirectToAction("StudentDashboard", "Dashboard");
                else
                    return RedirectToAction("Index");
            }
        }

        private bool PaymentExists(int id)
        {
            return _context.Payments.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Pay()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.Include(s => s.Room).FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null || student.Room == null)
            {
                TempData["ErrorMessage"] = "You must be assigned to a room to pay.";
                return RedirectToAction("StudentDashboard", "Dashboard");
            }

            var serviceCharge = 500M;
            var dueAmount = student.Room.MonthlyRent + serviceCharge;
            var currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            
            // Check if already paid for this month
            var alreadyPaid = await _context.Payments.AnyAsync(p => 
                p.StudentId == student.Id && 
                p.PaymentDate.Year == currentMonth.Year && 
                p.PaymentDate.Month == currentMonth.Month && 
                p.Status == PaymentStatus.Completed);

            var viewModel = new StudentPaymentViewModel
            {
                StudentName = student.FullName,
                StudentId = student.StudentId,
                RoomNumber = student.Room.RoomNumber,
                RoomRent = student.Room.MonthlyRent,
                ServiceCharge = serviceCharge,
                TotalDue = dueAmount,
                Amount = dueAmount,
                AlreadyPaid = alreadyPaid
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayConfirm(StudentPaymentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Pay", model);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students.Include(s => s.Room).FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null || student.Room == null)
            {
                TempData["ErrorMessage"] = "You must be assigned to a room to pay.";
                return RedirectToAction("StudentDashboard", "Dashboard");
            }

            var currentMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            // Prevent duplicate payment for the same month
            var alreadyPaid = await _context.Payments.AnyAsync(p => 
                p.StudentId == student.Id && 
                p.PaymentDate.Year == currentMonth.Year && 
                p.PaymentDate.Month == currentMonth.Month && 
                p.Status == PaymentStatus.Completed);

            if (alreadyPaid)
            {
                TempData["ErrorMessage"] = "You have already paid for this month.";
                return RedirectToAction("Pay");
            }

            var payment = new Payment
            {
                Amount = model.Amount,
                Status = PaymentStatus.Completed,
                Type = PaymentType.MobilePayment,
                HostelId = student.HostelId,
                StudentId = student.Id,
                RoomId = student.RoomId,
                PaymentDate = DateTime.Now,
                PaidAt = DateTime.Now,
                DueDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1),
                ReceiptNumber = $"RCPT-{DateTime.Now:yyyyMMddHHmmss}-{student.Id}",
                PaymentMethod = "Mobile Payment",
                Notes = $"Monthly payment for {DateTime.Now:MMMM yyyy} via mobile number {model.MobileNumber}",
                CreatedBy = userId
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Payment successful! <a href='/Payment/GenerateReceipt/{payment.Id}' target='_blank' class='btn btn-sm btn-info ms-2'>Download Receipt</a>";
            return RedirectToAction("StudentDashboard", "Dashboard");
        }
    }
} 