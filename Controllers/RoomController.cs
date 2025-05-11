using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Hostel2._0.Controllers
{
    [Authorize(Roles = "HostelManager")]
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
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

            var rooms = await _context.Rooms
                .Include(r => r.Occupants)
                .Where(r => r.HostelId == hostel.Id)
                .ToListAsync();

            return View(rooms);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room room)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
                
                if (hostel == null)
                {
                    return NotFound("Hostel not found");
                }

                room.HostelId = hostel.Id;
                room.IsAvailable = true;
                room.CurrentOccupancy = 0;
                room.CreatedAt = DateTime.UtcNow;
                room.CreatedBy = userId;

                _context.Add(room);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(room);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || room.HostelId != hostel.Id)
            {
                return NotFound("Room not found in your hostel");
            }

            return View(room);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room room)
        {
            if (id != room.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
                    
                    if (hostel == null || room.HostelId != hostel.Id)
                    {
                        return NotFound("Room not found in your hostel");
                    }

                    room.UpdatedAt = DateTime.UtcNow;
                    room.UpdatedBy = userId;
                    _context.Update(room);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.Id))
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
            return View(room);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Occupants)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || room.HostelId != hostel.Id)
            {
                return NotFound("Room not found in your hostel");
            }

            return View(room);
        }

        [HttpGet]
        [Route("Assign/{id}")]
        public async Task<IActionResult> Assign(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Occupants)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || room.HostelId != hostel.Id)
            {
                return NotFound("Room not found in your hostel");
            }

            var availableStudents = await _context.Students
                .Where(s => s.HostelId == hostel.Id && s.RoomId == null)
                .ToListAsync();

            ViewBag.AvailableStudents = availableStudents;
            return View(room);
        }

        [HttpPost]
        [Route("Assign/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(int id, int studentId)
        {
            var room = await _context.Rooms
                .Include(r => r.Occupants)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                return NotFound("Student not found");
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || room.HostelId != hostel.Id || student.HostelId != hostel.Id)
            {
                return NotFound("Invalid hostel assignment");
            }

            if (room.Occupants.Count >= room.Capacity)
            {
                ModelState.AddModelError("", "Room is at full capacity");
                return View(room);
            }

            student.RoomId = room.Id;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = room.Id });
        }

        private bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }
    }
} 