using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hostel2._0.Models.Enums;
using System.Collections.Generic;

namespace Hostel2._0.Models.MealManagement
{
    public class Meal : BaseEntity
    {
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
        public DateTime MealDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        public MealType Type { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Capacity { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int CurrentAttendance { get; set; }

        [Required]
        [Range(0, 10000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Alias for backward compatibility
        public decimal Rate => Price;

        public bool IsActive { get; set; }

        [StringLength(450)]
        public string? CreatedBy { get; set; }

        [StringLength(450)]
        public string? UpdatedBy { get; set; }

        public ICollection<StudentMeal> StudentMeals { get; set; } = new List<StudentMeal>();
    }
} 