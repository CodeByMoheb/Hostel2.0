using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models.MealManagement
{
    public class MealPayment : BaseEntity
    {
        [ForeignKey("StudentMeal")]
        public int StudentMealId { get; set; }
        public StudentMeal? StudentMeal { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        
        [Required]
        public PaymentStatus Status { get; set; }
        
        [Required]
        public PaymentType Type { get; set; }
        
        public DateTime? PaymentDate { get; set; }
        
        public string? TransactionId { get; set; }
        
        // Alias for views
        public string? TransactionReference => TransactionId;
        
        // Alias for views
        public PaymentType PaymentType => Type;
        
        [Required]
        public DateTime Month { get; set; }
        
        [Required]
        public int HostelId { get; set; }
        public virtual Hostel? Hostel { get; set; }

        public MealPayment()
        {
            Status = PaymentStatus.Pending;
            Type = PaymentType.Cash;
            Month = DateTime.UtcNow;
        }
    }
} 