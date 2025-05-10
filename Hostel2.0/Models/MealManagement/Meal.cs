using System.ComponentModel.DataAnnotations;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models.MealManagement
{
    public class Meal : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
        
        [Required]
        public MealType Type { get; set; }
        
        [Required]
        [Range(0.01, 1000)]
        [Display(Name = "Meal Rate")]
        public decimal Rate { get; set; }
        
        [Display(Name = "Description")]
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public new DateTime CreatedAt { get; set; } = DateTime.Now;
        public new DateTime UpdatedAt { get; set; } = DateTime.Now;
        
        // Navigation properties
        [Required]
        public required string HostelId { get; set; }
        public virtual Hostel? Hostel { get; set; }
    }
} 