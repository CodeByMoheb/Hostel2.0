using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Hostel2._0.Models.MealManagement
{
    public class MealPlan : BaseEntity
    {
        public MealPlan()
        {
            StudentMeals = new List<StudentMeal>();
        }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [ForeignKey("Hostel")]
        public int HostelId { get; set; }
        public Hostel? Hostel { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BreakfastRate { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal LunchRate { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal DinnerRate { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Duration { get; set; } = 30; // Duration in days
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        public DateTime EndDate { get; set; }
        
        public bool IsActive { get; set; }
        
        [StringLength(450)]
        public string? CreatedBy { get; set; }
        
        [StringLength(450)]
        public string? UpdatedBy { get; set; }
        
        public ICollection<StudentMeal> StudentMeals { get; set; }
    }
} 