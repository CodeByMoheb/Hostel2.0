using System.ComponentModel.DataAnnotations;
using Hostel2._0.Models.Enums;

namespace Hostel2._0.Models.MealManagement
{
    public class MealPayment : BaseEntity
    {
        [Required]
        public required string StudentId { get; set; }
        public virtual ApplicationUser? Student { get; set; }
        
        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public DateTime PaymentDate { get; set; }
        
        [Required]
        public PaymentType PaymentType { get; set; }
        
        [Required]
        public PaymentStatus Status { get; set; }
        
        public string? TransactionReference { get; set; }
        
        public string? Notes { get; set; }
        
        [Required]
        public DateTime Month { get; set; }
        
        [Required]
        public required string HostelId { get; set; }
        public virtual Hostel? Hostel { get; set; }
    }
} 