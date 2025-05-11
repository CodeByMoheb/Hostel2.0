using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Hostel2._0.Data;
using Hostel2._0.Models;
using Hostel2._0.Models.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Security.Claims;

namespace Hostel2._0.Controllers
{
    [Authorize(Roles = "HostelManager")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null)
            {
                return NotFound("Hostel not found");
            }

            var students = await _context.Students
                .Include(s => s.Room)
                .Include(s => s.User)
                .Where(s => s.HostelId == hostel.Id)
                .ToListAsync();

            return View(students);
        }

        public IActionResult Create()
        {
            return View(new StudentCreateViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
                
                if (hostel == null)
                {
                    return NotFound("Hostel not found");
                }

                // Generate username and password
                var username = $"{viewModel.FirstName.ToLower()}.{viewModel.LastName.ToLower()}";
                var password = GenerateRandomPassword();

                // Create ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = username,
                    Email = viewModel.Email,
                    PhoneNumber = viewModel.PhoneNumber,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Student");

                    // Create Student record
                    var student = new Student
                    {
                        FirstName = viewModel.FirstName,
                        LastName = viewModel.LastName,
                        Email = viewModel.Email,
                        PhoneNumber = viewModel.PhoneNumber,
                        StudentId = viewModel.StudentId,
                        DateOfBirth = viewModel.DateOfBirth,
                        Gender = viewModel.Gender,
                        Address = viewModel.Address,
                        HostelId = hostel.Id,
                        UserId = user.Id,
                        IsActive = true,
                        IsApproved = true,
                        IsManagerCreated = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = userId,
                        Name = $"{viewModel.FirstName} {viewModel.LastName}"
                    };

                    _context.Add(student);
                    await _context.SaveChangesAsync();

                    // Store credentials in ViewBag for display
                    TempData["Username"] = username;
                    TempData["Password"] = password;
                    TempData["SuccessMessage"] = "Student created successfully!";

                    return RedirectToAction(nameof(StudentCredentials));
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(viewModel);
        }

        public IActionResult StudentCredentials()
        {
            if (TempData["Username"] == null || TempData["Password"] == null)
            {
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Username = TempData["Username"].ToString();
            ViewBag.Password = TempData["Password"].ToString();
            ViewBag.SuccessMessage = TempData["SuccessMessage"]?.ToString();

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "HostelManager")]
        public async Task<IActionResult> ApproveStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || student.HostelId != hostel.Id)
            {
                return NotFound("Student not found in your hostel");
            }

            student.IsApproved = true;
            student.UpdatedAt = DateTime.UtcNow;
            student.UpdatedBy = userId;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private string GenerateRandomPassword()
        {
            // Define character sets
            const string upperCase = "ABCDEFGHJKLMNPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijkmnpqrstuvwxyz";
            const string numbers = "23456789";
            const string special = "!@#$%^&*";
            
            var random = new Random();
            var password = new char[12]; // Increased length for better security
            
            // Ensure at least one character from each required set
            password[0] = upperCase[random.Next(upperCase.Length)];
            password[1] = lowerCase[random.Next(lowerCase.Length)];
            password[2] = numbers[random.Next(numbers.Length)];
            password[3] = special[random.Next(special.Length)];
            
            // Fill the rest with a mix of all characters
            var allChars = upperCase + lowerCase + numbers + special;
            for (int i = 4; i < password.Length; i++)
            {
                password[i] = allChars[random.Next(allChars.Length)];
            }
            
            // Shuffle the password
            for (int i = password.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                var temp = password[i];
                password[i] = password[j];
                password[j] = temp;
            }
            
            return new string(password);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Room)
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || student.HostelId != hostel.Id)
            {
                return NotFound("Student not found in your hostel");
            }

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
                    
                    if (hostel == null || student.HostelId != hostel.Id)
                    {
                        return NotFound("Student not found in your hostel");
                    }

                    student.UpdatedAt = DateTime.UtcNow;
                    student.UpdatedBy = userId;
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .Include(s => s.Room)
                .Include(s => s.User)
                .Include(s => s.Payments)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || student.HostelId != hostel.Id)
            {
                return NotFound("Student not found in your hostel");
            }

            return View(student);
        }

        [HttpGet]
        [Route("AssignRoom/{id}")]
        public async Task<IActionResult> AssignRoom(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || student.HostelId != hostel.Id)
            {
                return NotFound("Student not found in your hostel");
            }

            var availableRooms = await _context.Rooms
                .Include(r => r.Occupants)
                .Where(r => r.HostelId == hostel.Id && r.Occupants.Count < r.Capacity)
                .ToListAsync();

            ViewBag.AvailableRooms = availableRooms;
            return View(student);
        }

        [HttpPost]
        [Route("AssignRoom/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRoom(int id, int roomId)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            var room = await _context.Rooms
                .Include(r => r.Occupants)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
            {
                return NotFound("Room not found");
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || student.HostelId != hostel.Id || room.HostelId != hostel.Id)
            {
                return NotFound("Invalid hostel assignment");
            }

            if (room.Occupants.Count >= room.Capacity)
            {
                ModelState.AddModelError("", "Room is at full capacity");
                return View(student);
            }

            student.RoomId = room.Id;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = student.Id });
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
} 