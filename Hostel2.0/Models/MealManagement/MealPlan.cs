using System.ComponentModel.DataAnnotations;

namespace Hostel2._0.Models.MealManagement
{
    public class MealPlan : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        
        [Required]
        [Range(0.01, 10.0)]
        public decimal BreakfastRate { get; set; }
        
        [Required]
        [Range(0.01, 20.0)]
        public decimal LunchRate { get; set; }
        
        [Required]
        [Range(0.01, 20.0)]
        public decimal DinnerRate { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        [Required]
        public required string HostelId { get; set; }
        public virtual Hostel? Hostel { get; set; }
    }
} 