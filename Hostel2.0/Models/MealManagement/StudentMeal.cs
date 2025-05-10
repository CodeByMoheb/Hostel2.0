using System.ComponentModel.DataAnnotations;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models.MealManagement
{
    public class StudentMeal : BaseEntity
    {
        [Required]
        public new string Id { get; set; }
        
        [Required]
        public required string StudentId { get; set; }
        public virtual ApplicationUser? Student { get; set; }
        
        [Required]
        public required string MealId { get; set; }
        public virtual Meal? Meal { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Meal Date")]
        public DateTime MealDate { get; set; } = DateTime.Today;
        
        [Required]
        public MealType Type { get; set; }
        
        [Display(Name = "Is Consumed")]
        public bool IsConsumed { get; set; } = true;
        
        [Required]
        [Range(0.01, 1000)]
        [Display(Name = "Rate")]
        public decimal Rate { get; set; }
        
        public bool IsPaid { get; set; } = false;
        
        public new DateTime CreatedAt { get; set; } = DateTime.Now;
    }
} 