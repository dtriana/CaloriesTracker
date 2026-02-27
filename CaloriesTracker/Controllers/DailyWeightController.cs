using CaloriesTracker.Data;
using CaloriesTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CaloriesTracker.Controllers
{
    [Authorize]
    public class DailyWeightController : Controller
    {
        private readonly AppDbContext _context;

        public DailyWeightController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var weights = await _context.DailyWeight
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.Date)
                .ToListAsync();

            return View(weights);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([FromForm] DateOnly date, [FromForm] decimal weigthInKg)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            if (date > DateOnly.FromDateTime(DateTime.Today))
            {
                ModelState.AddModelError("Date", "Date cannot be in the future");
            }

            if (weigthInKg <= 0)
            {
                ModelState.AddModelError("WeigthInKg", "Weight must be greater than zero");
            }

            if (!ModelState.IsValid)
            {
                var weights = await _context.DailyWeight
                    .Where(w => w.UserId == userId)
                    .OrderByDescending(w => w.Date)
                    .ToListAsync();
                return View("Index", weights);
            }

            var existing = await _context.DailyWeight
                .SingleOrDefaultAsync(w => w.UserId == userId && w.Date == date);

            if (existing != null)
            {
                existing.WeightInKg = weigthInKg;
                _context.DailyWeight.Update(existing);
            }
            else
            {
                var model = new DailyWeight
                {
                    Date = date,
                    WeightInKg = weigthInKg,
                    UserId = userId
                };

                _context.DailyWeight.Add(model);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
