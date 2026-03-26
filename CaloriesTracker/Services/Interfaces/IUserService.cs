using CaloriesTracker.Models.ViewModels.Account;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using CaloriesTracker.Models;
using CaloriesTracker.Models.Stats;

namespace CaloriesTracker.Services.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResult> RegisterAsync(RegisterViewModel model);
        Task<SignInResult> LoginAsync(string email, string password, bool rememberMe);
        Task LogoutAsync();
        Task<User?> GetUserAsync(string userId);
        Task<decimal> GetCurrentDailyGoalAsync(string userId);
        Task<IdentityResult> UpdateDailyGoalAsync(string userId, decimal newGoal);
        Task<IdentityResult> DeleteUserAsync(string userId);
        // Password reset helpers
        Task<(string userId, string token)?> GeneratePasswordResetTokenAsync(string email);
        Task<IdentityResult> ResetPasswordAsync(string userId, string token, string newPassword);
    }
}