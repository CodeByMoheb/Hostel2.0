using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hostel2._0.Data;
using Hostel2._0.Models;
using Hostel2._0.Models.Enums;
using Hostel2._0.Models.MealManagement;
using Hostel2._0.Models.ViewModels;
using System.Globalization;

namespace Hostel2._0.Controllers
{
    [Authorize(Roles = "HostelManager")]
    public class MealController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public MealController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Meal
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            // Find the hostel managed by the current user
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
            if (hostel == null) return NotFound("You are not assigned to manage any hostel.");

            var meals = await _context.Meals
                .Where(m => m.HostelId == hostel.Id)
                .OrderBy(m => m.Type)
                .ToListAsync();

            return View(meals);
        }

        // GET: Meal/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Meal/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MealCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) return NotFound();

                var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
                if (hostel == null) return NotFound("You are not assigned to manage any hostel.");

                var meal = new Meal
                {
                    Name = model.Name,
                    Type = model.Type,
                    Rate = model.Rate,
                    Description = model.Description,
                    HostelId = hostel.Id
                };

                _context.Add(meal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Meal/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meal = await _context.Meals.FindAsync(id);
            if (meal == null)
            {
                return NotFound();
            }

            // Check if user is authorized to edit this meal
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound("User not found");
            
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
            if (hostel == null || meal.HostelId != hostel.Id)
            {
                return Forbid();
            }

            var model = new MealEditViewModel
            {
                Id = meal.Id,
                Name = meal.Name,
                Type = meal.Type,
                Rate = meal.Rate,
                Description = meal.Description,
                IsActive = meal.IsActive
            };

            return View(model);
        }

        // POST: Meal/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, MealEditViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var meal = await _context.Meals.FindAsync(id);
                    if (meal == null)
                    {
                        return NotFound();
                    }

                    // Check if user is authorized to edit this meal
                    var currentUser = await _userManager.GetUserAsync(User);
                    if (currentUser == null) return NotFound("User not found");
                    
                    var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
                    if (hostel == null || meal.HostelId != hostel.Id)
                    {
                        return Forbid();
                    }

                    meal.Name = model.Name;
                    meal.Type = model.Type;
                    meal.Rate = model.Rate;
                    meal.Description = model.Description;
                    meal.IsActive = model.IsActive;
                    meal.UpdatedAt = DateTime.Now;

                    _context.Update(meal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MealExists(model.Id))
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
            return View(model);
        }

        // GET: Meal/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var meal = await _context.Meals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (meal == null)
            {
                return NotFound();
            }

            // Check if user is authorized to delete this meal
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound("User not found");
            
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
            if (hostel == null || meal.HostelId != hostel.Id)
            {
                return Forbid();
            }

            return View(meal);
        }

        // POST: Meal/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var meal = await _context.Meals.FindAsync(id);
            if (meal == null)
            {
                return NotFound();
            }

            // Check if user is authorized to delete this meal
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound("User not found");
            
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
            if (hostel == null || meal.HostelId != hostel.Id)
            {
                return Forbid();
            }

            _context.Meals.Remove(meal);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Meal/MealPlan
        public async Task<IActionResult> MealPlan()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
            if (hostel == null) return NotFound("You are not assigned to manage any hostel.");

            // Get current active meal plan or create a new one
            var mealPlan = await _context.MealPlans
                .Where(mp => mp.HostelId == hostel.Id && mp.IsActive)
                .OrderByDescending(mp => mp.CreatedAt)
                .FirstOrDefaultAsync();

            if (mealPlan == null)
            {
                var model = new MealPlanViewModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Now.Month)} {DateTime.Now.Year} Meal Plan"
                };
                return View(model);
            }

            var viewModel = new MealPlanViewModel
            {
                Id = mealPlan.Id,
                Name = mealPlan.Name,
                BreakfastRate = mealPlan.BreakfastRate,
                LunchRate = mealPlan.LunchRate,
                DinnerRate = mealPlan.DinnerRate,
                StartDate = mealPlan.StartDate,
                EndDate = mealPlan.EndDate,
                IsActive = mealPlan.IsActive
            };

            return View(viewModel);
        }

        // POST: Meal/MealPlan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MealPlan(MealPlanViewModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) return NotFound();

                var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
                if (hostel == null) return NotFound("You are not assigned to manage any hostel.");

                if (string.IsNullOrEmpty(model.Id))
                {
                    // Create new meal plan
                    var mealPlan = new MealPlan
                    {
                        Name = model.Name,
                        BreakfastRate = model.BreakfastRate,
                        LunchRate = model.LunchRate,
                        DinnerRate = model.DinnerRate,
                        StartDate = model.StartDate,
                        EndDate = model.EndDate,
                        IsActive = model.IsActive,
                        HostelId = hostel.Id
                    };

                    // If this is active, deactivate all other plans
                    if (model.IsActive)
                    {
                        var activePlans = await _context.MealPlans
                            .Where(mp => mp.HostelId == hostel.Id && mp.IsActive)
                            .ToListAsync();

                        foreach (var plan in activePlans)
                        {
                            plan.IsActive = false;
                            plan.UpdatedAt = DateTime.Now;
                            _context.Update(plan);
                        }
                    }

                    _context.Add(mealPlan);
                }
                else
                {
                    // Update existing meal plan
                    var mealPlan = await _context.MealPlans.FindAsync(model.Id);
                    if (mealPlan == null) return NotFound();

                    if (mealPlan.HostelId != hostel?.Id) return Forbid();

                    mealPlan.Name = model.Name;
                    mealPlan.BreakfastRate = model.BreakfastRate;
                    mealPlan.LunchRate = model.LunchRate;
                    mealPlan.DinnerRate = model.DinnerRate;
                    mealPlan.StartDate = model.StartDate;
                    mealPlan.EndDate = model.EndDate;
                    mealPlan.IsActive = model.IsActive;
                    mealPlan.UpdatedAt = DateTime.Now;

                    // If this is active, deactivate all other plans
                    if (model.IsActive)
                    {
                        var activePlans = await _context.MealPlans
                            .Where(mp => mp.HostelId == hostel.Id && mp.IsActive && mp.Id != model.Id)
                            .ToListAsync();

                        foreach (var plan in activePlans)
                        {
                            plan.IsActive = false;
                            plan.UpdatedAt = DateTime.Now;
                            _context.Update(plan);
                        }
                    }

                    _context.Update(mealPlan);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(MealPlan));
            }
            return View(model);
        }

        // GET: Meal/RecordMeals
        public async Task<IActionResult> RecordMeals(DateTime? date)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var hostel = await _context.Hostels
                .Include(h => h.Rooms)
                .FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
            
            if (hostel == null) return NotFound("You are not assigned to manage any hostel.");

            // Get all students in this hostel
            var students = await _userManager.Users
                .Where(u => u.HostelId == hostel.Id && u.Role == UserRole.Student && u.IsActive)
                .ToListAsync();

            // Get all active meals for this hostel
            var meals = await _context.Meals
                .Where(m => m.HostelId == hostel.Id && m.IsActive)
                .ToListAsync();

            var selectedDate = date ?? DateTime.Today;

            // Get meal plan for the selected date
            var mealPlan = await _context.MealPlans
                .Where(mp => mp.HostelId == hostel.Id && mp.StartDate <= selectedDate && mp.EndDate >= selectedDate && mp.IsActive)
                .FirstOrDefaultAsync();

            if (mealPlan == null)
            {
                // Default rates if no meal plan exists
                foreach (var meal in meals)
                {
                    switch (meal?.Type)
                    {
                        case MealType.Breakfast:
                            meal.Rate = 0.5m;
                            break;
                        case MealType.Lunch:
                            meal.Rate = 1.0m;
                            break;
                        case MealType.Dinner:
                            meal.Rate = 1.0m;
                            break;
                    }
                }
            }
            else
            {
                // Apply meal plan rates
                foreach (var meal in meals)
                {
                    if (meal != null)
                    {
                        decimal rate = meal.Rate;
                        
                        // Apply meal plan rates if available
                        if (mealPlan != null)
                        {
                            switch (meal.Type)
                            {
                                case MealType.Breakfast:
                                    rate = mealPlan.BreakfastRate;
                                    break;
                                case MealType.Lunch:
                                    rate = mealPlan.LunchRate;
                                    break;
                                case MealType.Dinner:
                                    rate = mealPlan.DinnerRate;
                                    break;
                            }
                        }

                        meal.Rate = rate;
                    }
                }
            }

            // Get existing student meals for the selected date
            var existingMeals = await _context.StudentMeals
                .Where(sm => sm.MealDate.Date == selectedDate.Date && 
                       sm.Student.HostelId == hostel.Id)
                .ToListAsync();

            var viewModel = new List<StudentMealViewModel>();
            
            foreach (var student in students)
            {
                var studentMeals = new StudentMealViewModel
                {
                    StudentId = student.Id,
                    StudentName = $"{student.FirstName} {student.LastName}",
                    MealDate = selectedDate,
                    Meals = meals.Select(m => new MealSelectionViewModel
                    {
                        MealId = m.Id,
                        Name = m.Name,
                        Type = m.Type,
                        Rate = m.Rate,
                        IsSelected = existingMeals.Any(em => em.StudentId == student.Id && em.MealId == m.Id)
                    }).ToList()
                };
                
                viewModel.Add(studentMeals);
            }

            ViewBag.SelectedDate = selectedDate;
            return View(viewModel);
        }

        // POST: Meal/RecordMeals
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordMeals(List<StudentMealViewModel> model, DateTime selectedDate)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
            if (hostel == null) return NotFound("You are not assigned to manage any hostel.");

            // Get all active meals for this hostel
            var meals = await _context.Meals
                .Where(m => m.HostelId == hostel.Id && m.IsActive)
                .ToListAsync();

            // Get meal plan for the selected date
            var mealPlan = await _context.MealPlans
                .Where(mp => mp.HostelId == hostel.Id && mp.StartDate <= selectedDate && mp.EndDate >= selectedDate && mp.IsActive)
                .FirstOrDefaultAsync();

            // Delete all existing meals for the date and hostel
            var existingMeals = await _context.StudentMeals
                .Where(sm => sm.MealDate.Date == selectedDate.Date && 
                       sm.Student.HostelId == hostel.Id)
                .ToListAsync();
            
            _context.StudentMeals.RemoveRange(existingMeals);

            // Add new meals
            foreach (var studentMeal in model)
            {
                foreach (var mealSelection in studentMeal.Meals.Where(m => m.IsSelected))
                {
                    var meal = meals.FirstOrDefault(m => m.Id == mealSelection.MealId);
                    if (meal == null) continue;

                    decimal rate = meal.Rate;
                    
                    // Apply meal plan rates if available
                    if (mealPlan != null)
                    {
                        switch (meal?.Type)
                        {
                            case MealType.Breakfast:
                                rate = mealPlan.BreakfastRate;
                                break;
                            case MealType.Lunch:
                                rate = mealPlan.LunchRate;
                                break;
                            case MealType.Dinner:
                                rate = mealPlan.DinnerRate;
                                break;
                        }
                    }

                    var newMeal = new StudentMeal
                    {
                        StudentId = studentMeal.StudentId,
                        MealId = mealSelection.MealId,
                        MealDate = selectedDate,
                        Rate = rate,
                        IsConsumed = true
                    };

                    _context.StudentMeals.Add(newMeal);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(RecordMeals), new { date = selectedDate });
        }

        // GET: Meal/MealReport
        public async Task<IActionResult> MealReport(DateTime? month)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound();

            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
            if (hostel == null) return NotFound("You are not assigned to manage any hostel.");

            // Get the month to report on
            var reportMonth = month.HasValue 
                ? new DateTime(month.Value.Year, month.Value.Month, 1)
                : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            
            // Month end date
            var monthEnd = reportMonth.AddMonths(1).AddDays(-1);

            // Get all students in this hostel
            var students = await _userManager.Users
                .Where(u => u.HostelId == hostel.Id && u.Role == UserRole.Student && u.IsActive)
                .ToListAsync();

            // Get all meals consumed in the month
            var mealRecords = await _context.StudentMeals
                .Include(sm => sm.Meal)
                .Include(sm => sm.Student)
                .Where(sm => sm.MealDate >= reportMonth && 
                       sm.MealDate <= monthEnd && 
                       sm.Student.HostelId == hostel.Id)
                .ToListAsync();

            // Get all meal payments in the month
            var mealPayments = await _context.MealPayments
                .Where(mp => mp.Month.Year == reportMonth.Year && 
                       mp.Month.Month == reportMonth.Month && 
                       mp.HostelId == hostel.Id && 
                       mp.Status == PaymentStatus.Completed)
                .ToListAsync();

            var report = new MealReportViewModel
            {
                Month = reportMonth,
                StudentReports = new List<StudentMealReportViewModel>()
            };

            foreach (var student in students)
            {
                var studentMeals = mealRecords.Where(m => m.StudentId == student.Id).ToList();
                var studentPayments = mealPayments.Where(p => p.StudentId == student.Id).ToList();
                
                var studentReport = new StudentMealReportViewModel
                {
                    StudentId = student.Id,
                    StudentName = $"{student.FirstName} {student.LastName}",
                    TotalMealCost = studentMeals.Sum(m => m.Rate),
                    TotalPaid = studentPayments.Sum(p => p.Amount),
                    BreakfastCount = studentMeals.Count(m => m.Meal?.Type == MealType.Breakfast),
                    LunchCount = studentMeals.Count(m => m.Meal?.Type == MealType.Lunch),
                    DinnerCount = studentMeals.Count(m => m.Meal?.Type == MealType.Dinner),
                    MealDetails = studentMeals
                };

                report.StudentReports.Add(studentReport);
            }

            ViewBag.CurrentMonth = reportMonth;
            ViewBag.PreviousMonth = reportMonth.AddMonths(-1);
            ViewBag.NextMonth = reportMonth.AddMonths(1) > DateTime.Now ? (DateTime?)null : reportMonth.AddMonths(1);

            return View(report);
        }

        // GET: Meal/StudentReport/5
        public async Task<IActionResult> StudentReport(string id, DateTime? month)
        {
            if (id == null) return NotFound();

            var student = await _userManager.FindByIdAsync(id);
            if (student == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound("User not found");
            
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
            if (hostel == null || student.HostelId != hostel.Id) return Forbid();

            // Get the month to report on
            var reportMonth = month.HasValue 
                ? new DateTime(month.Value.Year, month.Value.Month, 1)
                : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            
            // Month end date
            var monthEnd = reportMonth.AddMonths(1).AddDays(-1);

            // Get all meals consumed by the student in the month
            var mealRecords = await _context.StudentMeals
                .Include(sm => sm.Meal)
                .Where(sm => sm.StudentId == id && 
                       sm.MealDate >= reportMonth && 
                       sm.MealDate <= monthEnd)
                .OrderBy(sm => sm.MealDate)
                .ThenBy(sm => sm.Meal!.Type)
                .ToListAsync();

            // Get all meal payments by the student in the month
            var mealPayments = await _context.MealPayments
                .Where(mp => mp.StudentId == id && 
                       mp.Month.Year == reportMonth.Year && 
                       mp.Month.Month == reportMonth.Month && 
                       mp.Status == PaymentStatus.Completed)
                .ToListAsync();

            var studentReport = new StudentMealReportViewModel
            {
                StudentId = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                TotalMealCost = mealRecords.Sum(m => m.Rate),
                TotalPaid = mealPayments.Sum(p => p.Amount),
                BreakfastCount = mealRecords.Count(m => m.Meal?.Type == MealType.Breakfast),
                LunchCount = mealRecords.Count(m => m.Meal?.Type == MealType.Lunch),
                DinnerCount = mealRecords.Count(m => m.Meal?.Type == MealType.Dinner),
                MealDetails = mealRecords
            };

            ViewBag.Student = student;
            ViewBag.CurrentMonth = reportMonth;
            ViewBag.Payments = mealPayments;
            
            return View(studentReport);
        }

        // GET: Meal/RecordPayment/5
        public async Task<IActionResult> RecordPayment(string id)
        {
            if (id == null) return NotFound();

            var student = await _userManager.FindByIdAsync(id);
            if (student == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound("User not found");
            
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
            if (hostel == null || student.HostelId != hostel.Id) return Forbid();

            var model = new MealPaymentViewModel
            {
                StudentId = student.Id
            };

            ViewBag.Student = student;
            return View(model);
        }

        // POST: Meal/RecordPayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordPayment(MealPaymentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var student = await _userManager.FindByIdAsync(model.StudentId);
            if (student == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return NotFound("User not found");
            
            var hostel = await _context.Hostels.FirstOrDefaultAsync(h => h.ManagerId == currentUser.Id);
            if (hostel == null || student.HostelId != hostel.Id) return Forbid();

            var payment = new MealPayment
            {
                StudentId = model.StudentId,
                Amount = model.Amount,
                PaymentDate = DateTime.Today,
                PaymentType = model.PaymentType,
                Status = PaymentStatus.Completed,
                TransactionReference = model.TransactionReference,
                Notes = model.Notes,
                Month = model.Month,
                HostelId = hostel?.Id ?? throw new InvalidOperationException("Hostel ID cannot be null")
            };

            _context.MealPayments.Add(payment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(StudentReport), new { id = model.StudentId, month = model.Month });
        }

        private bool MealExists(string id)
        {
            return _context.Meals.Any(e => e.Id == id);
        }
    }
} 