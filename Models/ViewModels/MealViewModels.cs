using System.ComponentModel.DataAnnotations;
using Hostel2._0.Models.Enums;
using Hostel2._0.Models.MealManagement;

namespace Hostel2._0.Models.ViewModels
{
    public class MealCreateViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Meal Name")]
        public required string Name { get; set; }
        
        [Required]
        [Display(Name = "Meal Type")]
        public Enums.MealType Type { get; set; }
        
        [Required]
        [Display(Name = "Meal Rate")]
        [Range(0.1, 10.0)]
        public decimal Rate { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }
    
    public class MealEditViewModel : MealCreateViewModel
    {
        public required string Id { get; set; }
        
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
    
    public class MealPlanViewModel
    {
        public required string Id { get; set; }
        
        [Required]
        [StringLength(50)]
        [Display(Name = "Plan Name")]
        public required string Name { get; set; }
        
        [Required]
        [Range(0.1, 100.0)]
        [Display(Name = "Breakfast Rate")]
        public decimal BreakfastRate { get; set; } = 0.5m;
        
        [Required]
        [Range(0.1, 100.0)]
        [Display(Name = "Lunch Rate")]
        public decimal LunchRate { get; set; } = 1.0m;
        
        [Required]
        [Range(0.1, 100.0)]
        [Display(Name = "Dinner Rate")]
        public decimal DinnerRate { get; set; } = 1.0m;
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
        
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
    
    public class StudentMealViewModel
    {
        [Display(Name = "Student")]
        public required string StudentId { get; set; }
        public required string StudentName { get; set; }
        
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        public DateTime MealDate { get; set; } = DateTime.Today;
        
        public List<MealSelectionViewModel> Meals { get; set; } = new List<MealSelectionViewModel>();
    }
    
    public class MealSelectionViewModel
    {
        public required string MealId { get; set; }
        public required string Name { get; set; }
        public Enums.MealType Type { get; set; }
        public decimal Rate { get; set; }
        public bool IsSelected { get; set; } = false;
    }
    
    public class MealReportViewModel
    {
        public DateTime Month { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public List<StudentMealReportViewModel> StudentReports { get; set; } = new List<StudentMealReportViewModel>();
    }
    
    public class StudentMealReportViewModel
    {
        public required string StudentId { get; set; }
        public required string StudentName { get; set; }
        public decimal TotalMealCost { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Balance => TotalPaid - TotalMealCost;
        public int BreakfastCount { get; set; }
        public int LunchCount { get; set; }
        public int DinnerCount { get; set; }
        public int TotalMeals => BreakfastCount + LunchCount + DinnerCount;
        public List<StudentMeal> MealDetails { get; set; } = new List<StudentMeal>();
    }
    
    public class MealPaymentViewModel
    {
        public required string StudentId { get; set; }
        
        [Required]
        [Range(0.01, 100000)]
        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }
        
        [Required]
        [Display(Name = "Payment Type")]
        public PaymentType PaymentType { get; set; }
        
        [Display(Name = "Transaction Reference")]
        public string? TransactionReference { get; set; }
        
        [Display(Name = "Notes")]
        public string? Notes { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Month")]
        public DateTime Month { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
    }
} 