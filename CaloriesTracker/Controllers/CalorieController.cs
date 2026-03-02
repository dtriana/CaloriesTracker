using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CaloriesTracker.Models;
using CaloriesTracker.Services;
using System.Security.Claims;


namespace CaloriesTracker.Controllers
{
    [Authorize]
    public class CalorieController(CalorieService calorieService, FoodService productService) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(DateOnly? date)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var viewDate = date ?? DateOnly.FromDateTime(DateTime.Today);
            var summary = await calorieService.GetDailySummaryAsync(userId, viewDate,0);
            var foods = await productService.GetUserProductsAsync(userId);

            ViewBag.Date = viewDate;
            ViewBag.Foods = foods;

            return View(summary);
        }

        [HttpPost]
        public async Task<IActionResult> RemoveIntake(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            await calorieService.RemoveIntakeAsync(id);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> AddIntakeWithDate(int productId, decimal quantity, DateOnly intakeDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            if (intakeDate > DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError("intakeDate", "Date cannot be in the future");
                return View("Index", await calorieService.GetDailySummaryAsync(userId, intakeDate, 0));

            }
            await calorieService.AddIntakeAsync(userId, productId, quantity, intakeDate);
            return RedirectToAction("Index", new { date = intakeDate });

        }


        [HttpPost]
        public async Task<IActionResult> EditIntakeQuantity(int id, decimal quantity)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            await calorieService.UpdateIntakeQuantityAsync(id, quantity);
            return RedirectToAction("Index");
        }

    }
}
