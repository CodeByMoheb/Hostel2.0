using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Hostel2._0.Controllers
{
    [Authorize(Roles = "Manager")]
    public class DocumentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DocumentController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null)
            {
                return NotFound("Hostel not found");
            }

            var documents = await _context.Documents
                .Include(d => d.Student)
                .Where(d => d.HostelId == hostel.Id)
                .OrderByDescending(d => d.UploadDate)
                .ToListAsync();

            return View(documents);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Document document, Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (ModelState.IsValid && file != null)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
                
                if (hostel == null)
                {
                    return NotFound("Hostel not found");
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                document.HostelId = hostel.Id;
                document.FileName = uniqueFileName;
                document.FileType = Path.GetExtension(file.FileName);
                document.FileSize = file.Length;
                document.UploadDate = DateTime.Now;
                document.IsVerified = false;

                _context.Add(document);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(document);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents
                .Include(d => d.Student)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || document.HostelId != hostel.Id)
            {
                return NotFound("Document not found in your hostel");
            }

            return View(document);
        }

        public async Task<IActionResult> Download(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || document.HostelId != hostel.Id)
            {
                return NotFound("Document not found in your hostel");
            }

            var filePath = Path.Combine(_environment.WebRootPath, "uploads", document.FileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found");
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, "application/octet-stream", document.FileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || document.HostelId != hostel.Id)
            {
                return NotFound("Document not found in your hostel");
            }

            document.IsVerified = true;
            document.VerifiedAt = DateTime.Now;
            document.VerifiedBy = userId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = document.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var document = await _context.Documents.FindAsync(id);
            if (document == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || document.HostelId != hostel.Id)
            {
                return NotFound("Document not found in your hostel");
            }

            var filePath = Path.Combine(_environment.WebRootPath, "uploads", document.FileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            _context.Documents.Remove(document);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentExists(int id)
        {
            return _context.Documents.Any(e => e.Id == id);
        }
    }
} 