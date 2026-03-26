using CaloriesTracker.Models;
using CaloriesTracker.Models.ViewModels.Account;
using CaloriesTracker.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CaloriesTracker.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly Services.Interfaces.IEmailSender _emailSender;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger, Services.Interfaces.IEmailSender emailSender)
        {
            _userService = userService;
            _logger = logger;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register(decimal? calories = null)
        {
            var model = new RegisterViewModel
            {
                DailyCalorieGoal = calories ?? 2000m
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _userService.RegisterAsync(model);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} registered.", model.Email);
                await _userService.LoginAsync(model.Email, model.Password, rememberMe: false);
                return RedirectToHome();
            }

            AddErrors(result.Errors, model.Email);
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl)
        {
            TempData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new Models.ViewModels.Account.ForgotPasswordViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(Models.ViewModels.Account.ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var info = await _userService.GeneratePasswordResetTokenAsync(model.Email);
            // Always show confirmation regardless of whether user exists
            if (info != null)
            {
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = info.Value.userId, token = System.Net.WebUtility.UrlEncode(info.Value.token) }, Request.Scheme!);
                var html = $"Please reset your password by <a href='{callbackUrl}'>clicking here</a>.";
                await _emailSender.SendEmailAsync(model.Email, "Reset Password", html);
                _logger.LogInformation("Password reset email queued for {Email}.", model.Email);
            }

            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            var vm = new Models.ViewModels.Account.ResetPasswordViewModel { UserId = userId, Token = token };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(Models.ViewModels.Account.ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var decodedToken = System.Net.WebUtility.UrlDecode(model.Token);
            var result = await _userService.ResetPasswordAsync(model.UserId, decodedToken!, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} successfully reset password.", model.UserId);
                return View("ResetPasswordConfirmation");
            }

            AddErrors(result.Errors, model.UserId);
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            string? returnUrl = TempData["ReturnUrl"] as string;
            if (!ModelState.IsValid) return View(model);

            var result = await _userService.LoginAsync(model.Email, model.Password, model.RememberMe);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {Email} logged in.", model.Email);
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty,
                    "Account is locked due to multiple failed login attempts.");
                _logger.LogWarning("Lockout for {Email}.", model.Email);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                _logger.LogWarning("Invalid login attempt for {Email}.", model.Email);
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _userService.LogoutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToHome();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? userId = GetUserId();
            if (userId == null) return Unauthorized();

            var goal = await _userService.GetCurrentDailyGoalAsync(userId);
            var vm = new AccountSettingsViewModel { DailyCalorieGoal = goal };
            return View(vm);
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateDailyGoal(AccountSettingsViewModel model)
        {
            if (!ModelState.IsValid) return View("Index", model);

            string? userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _userService.UpdateDailyGoalAsync(userId, model.DailyCalorieGoal);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserId} updated daily goal to {Goal}.",
                    userId, model.DailyCalorieGoal);
                return RedirectToHome();
            }

            AddErrors(result.Errors, userId);
            return View("Index", model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult DeleteAccount() => View();

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAccountConfirmed()
        {
            string? userId = GetUserId();
            if (userId == null) return Unauthorized();

            var result = await _userService.DeleteUserAsync(userId);
            if (result.Succeeded)
            {
                await _userService.LogoutAsync();
                _logger.LogInformation("User {UserId} deleted their account.", userId);
                return RedirectToHome();
            }

            AddErrors(result.Errors, userId);
            return View("DeleteAccount");
        }
        private string? GetUserId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        private IActionResult RedirectToHome() =>
            RedirectToAction(nameof(HomeController.Index), "Home");

        private IActionResult RedirectToLocal(string? returnUrl) =>
            Url.IsLocalUrl(returnUrl) ? Redirect(returnUrl) : RedirectToHome();

        private void AddErrors(IEnumerable<IdentityError> errors, string keyContext)
        {
            foreach (var err in errors)
            {
                ModelState.AddModelError(string.Empty, err.Description);
                _logger.LogWarning("Error ({Context}): {Description}", keyContext, err.Description);
            }
        }
    }
}
