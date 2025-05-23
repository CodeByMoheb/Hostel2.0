using System.ComponentModel.DataAnnotations;

namespace Hostel2._0.Models.ViewModels.PaymentViewModels
{
    public class StudentPaymentViewModel
    {
        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^01[3-9]\d{8}$", ErrorMessage = "Please enter a valid Bangladeshi mobile number")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment amount is required")]
        [Range(1, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Display(Name = "Payment Amount")]
        public decimal Amount { get; set; }

        public string StudentName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public decimal RoomRent { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal TotalDue { get; set; }
        public bool AlreadyPaid { get; set; }
    }
} 