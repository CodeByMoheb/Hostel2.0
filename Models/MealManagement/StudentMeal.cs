using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hostel2._0.Models.Enums;
using System.Collections.Generic;

namespace Hostel2._0.Models.MealManagement
{
    public class StudentMeal : BaseEntity
    {
        public StudentMeal()
        {
            Payments = new List<MealPayment>();
        }

        [Required]
        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        
        [Required]
        [ForeignKey("Meal")]
        public int MealId { get; set; }
        public Meal? Meal { get; set; }
        
        [Required]
        public DateTime MealDate { get; set; }
        
        public bool IsAttended { get; set; }
        public DateTime? AttendedAt { get; set; }
        
        [StringLength(500)]
        public string? Notes { get; set; }
        
        [ForeignKey("MealPlan")]
        public int? MealPlanId { get; set; }
        public MealPlan? MealPlan { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Meal Date")]
        public DateTime Date { get; set; }
        
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
        
        public ICollection<MealPayment> Payments { get; set; }
    }
} 