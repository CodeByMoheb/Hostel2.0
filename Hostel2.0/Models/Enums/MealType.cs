using System.ComponentModel.DataAnnotations;

namespace Hostel2._0.Models.Enums
{
    public enum MealType
    {
        [Display(Name = "Breakfast")]
        Breakfast = 0,
        
        [Display(Name = "Lunch")]
        Lunch = 1,
        
        [Display(Name = "Dinner")]
        Dinner = 2
    }
} 