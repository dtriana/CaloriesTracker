using System.ComponentModel.DataAnnotations;

namespace CaloriesTracker.Models.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
