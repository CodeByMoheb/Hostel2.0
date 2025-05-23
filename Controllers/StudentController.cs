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
using Microsoft.AspNetCore.Mvc.Rendering;
using Hostel2._0.Models.Enums;

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

            ViewBag.Username = TempData["Username"]?.ToString() ?? "N/A";
            ViewBag.Password = TempData["Password"]?.ToString() ?? "N/A";
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

            // Get available rooms for the hostel
            var availableRooms = await _context.Rooms
                .Where(r => r.HostelId == hostel.Id)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = $"{r.RoomNumber} - {r.Type} (Capacity: {r.Students.Count}/{r.Capacity})"
                })
                .ToListAsync();

            ViewBag.Rooms = availableRooms;

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,StudentId,DateOfBirth,Gender,Address,RoomId,IsActive,HostelId,UserId,CreatedAt,CreatedBy")] Student student)
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

                    // Get the existing student record
                    var existingStudent = await _context.Students
                        .Include(s => s.User)
                        .FirstOrDefaultAsync(s => s.Id == id);

                    if (existingStudent == null)
                    {
                        return NotFound();
                    }

                    // Update only editable fields
                    existingStudent.FirstName = student.FirstName;
                    existingStudent.LastName = student.LastName;
                    existingStudent.Name = $"{student.FirstName} {student.LastName}";
                    existingStudent.Email = student.Email;
                    existingStudent.PhoneNumber = student.PhoneNumber;
                    existingStudent.StudentId = student.StudentId;
                    existingStudent.DateOfBirth = student.DateOfBirth;
                    existingStudent.Gender = student.Gender;
                    existingStudent.Address = student.Address;
                    existingStudent.RoomId = student.RoomId;
                    existingStudent.IsActive = student.IsActive;
                    existingStudent.UpdatedAt = DateTime.UtcNow;
                    existingStudent.UpdatedBy = userId;

                    // Update the associated user's email and phone number
                    if (existingStudent.User != null)
                    {
                        existingStudent.User.Email = student.Email;
                        existingStudent.User.PhoneNumber = student.PhoneNumber;
                        await _userManager.UpdateAsync(existingStudent.User);
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Student information updated successfully.";
                    return RedirectToAction(nameof(Index));
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
            }

            // If we got this far, something failed, redisplay form
            var availableRooms = await _context.Rooms
                .Where(r => r.HostelId == student.HostelId)
                .Select(r => new SelectListItem
                {
                    Value = r.Id.ToString(),
                    Text = $"{r.RoomNumber} - {r.Type} (Capacity: {r.Students.Count}/{r.Capacity})"
                })
                .ToListAsync();

            ViewBag.Rooms = availableRooms;
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
                .Include(r => r.Students)
                .Where(r => r.HostelId == hostel.Id && r.Students.Count < r.Capacity)
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
                .Include(r => r.Students)
                .FirstOrDefaultAsync(r => r.Id == roomId);

            if (room == null)
            {
                return NotFound();
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == userId);
            
            if (hostel == null || student.HostelId != hostel.Id || room.HostelId != hostel.Id)
            {
                return NotFound("Invalid hostel assignment");
            }

            if (room.Students.Count >= room.Capacity)
            {
                ModelState.AddModelError("", "Room is at full capacity");
                return View(room);
            }

            student.RoomId = room.Id;
            await _context.SaveChangesAsync();

            // Create pending payment for the current month if not exists
            var now = DateTime.Now;
            var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.StudentId == student.Id && p.PaymentDate.Year == now.Year && p.PaymentDate.Month == now.Month);
            if (existingPayment == null)
            {
                var serviceCharge = 500M;
                var dueAmount = room.MonthlyRent + serviceCharge;
                var payment = new Payment
                {
                    Amount = dueAmount,
                    Status = PaymentStatus.Pending,
                    Type = PaymentType.Cash,
                    HostelId = student.HostelId,
                    StudentId = student.Id,
                    RoomId = room.Id,
                    PaymentDate = now,
                    DueDate = new DateTime(now.Year, now.Month, 1).AddMonths(1).AddDays(-1),
                    ReceiptNumber = $"RCPT-{now:yyyyMMddHHmmss}-{student.Id}",
                    PaymentMethod = "Cash",
                    Notes = $"Monthly payment for {now:MMMM yyyy}",
                    CreatedBy = userId
                };
                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = student.Id });
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Profile()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var student = await _context.Students
                .Include(s => s.Room)
                .FirstOrDefaultAsync(s => s.UserId == userId);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.Id == id);
        }
    }
} 