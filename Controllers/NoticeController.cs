using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Hostel2._0.Controllers
{
    [Authorize(Roles = "HostelManager")]
    public class NoticeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NoticeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "HostelManager,Student")]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null)
            {
                return NotFound("Hostel not found");
            }

            var notices = await _context.Notices
                .Include(n => n.Recipients)
                    .ThenInclude(r => r.Student)
                .Where(n => n.HostelId == hostel.Id)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notices);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Notice notice)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
                
                if (hostel == null)
                {
                    return NotFound("Hostel not found");
                }

                notice.HostelId = hostel.Id;
                notice.CreatedAt = DateTime.UtcNow;
                notice.CreatedBy = userId;
                notice.IsActive = true;

                _context.Add(notice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notice);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notice = await _context.Notices
                .Include(n => n.Recipients)
                    .ThenInclude(r => r.Student)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notice == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || notice.HostelId != hostel.Id)
            {
                return NotFound("Notice not found in your hostel");
            }

            return View(notice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Notice notice)
        {
            if (id != notice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
                    
                    if (hostel == null || notice.HostelId != hostel.Id)
                    {
                        return NotFound("Notice not found in your hostel");
                    }

                    notice.UpdatedAt = DateTime.UtcNow;
                    notice.UpdatedBy = userId;
                    _context.Update(notice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NoticeExists(notice.Id))
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
            return View(notice);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notice = await _context.Notices
                .Include(n => n.Recipients)
                    .ThenInclude(r => r.Student)
                .FirstOrDefaultAsync(n => n.Id == id);

            if (notice == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || notice.HostelId != hostel.Id)
            {
                return NotFound("Notice not found in your hostel");
            }

            return View(notice);
        }

        public async Task<IActionResult> AssignRecipients(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notice = await _context.Notices.FindAsync(id);
            if (notice == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || notice.HostelId != hostel.Id)
            {
                return NotFound("Notice not found in your hostel");
            }

            var students = await _context.Students
                .Where(s => s.HostelId == hostel.Id)
                .ToListAsync();

            ViewBag.Students = students;
            return View(notice);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRecipients(int id, int[] studentIds)
        {
            var notice = await _context.Notices.FindAsync(id);
            if (notice == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || notice.HostelId != hostel.Id)
            {
                return NotFound("Notice not found in your hostel");
            }

            foreach (var studentId in studentIds)
            {
                var student = await _context.Students.FindAsync(studentId);
                if (student != null && student.HostelId == hostel.Id)
                {
                    var recipient = new NoticeRecipient
                    {
                        NoticeId = notice.Id,
                        StudentId = studentId,
                        IsRead = false
                    };
                    _context.Add(recipient);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = notice.Id });
        }

        private bool NoticeExists(int id)
        {
            return _context.Notices.Any(e => e.Id == id);
        }
    }
} 