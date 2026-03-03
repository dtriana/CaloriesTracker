using CaloriesTracker.Data;
using CaloriesTracker.Models;
using CaloriesTracker.Models.Stats;
using CaloriesTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CaloriesTracker.Services
{
        // Інтерфейс для отримання поточного часу (для кращої тестованості)
        public interface IDateTimeProvider
        {
            DateTime Now { get; }
        }

        // Реалізація інтерфейсу за замовчуванням — повертає системний час
        public class SystemDateTimeProvider : IDateTimeProvider
        {
            public DateTime Now => DateTime.Now;
        }

    public class CalorieService
    {
        private readonly AppDbContext _context;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IDailySummaryService _summaryService;
        private readonly IUserStatsService _userStatsService;

        public CalorieService(
            AppDbContext context,
            IDateTimeProvider dateTimeProvider,
            IDailySummaryService summaryService,
            IUserStatsService userStatsService)
        {
            _context = context;
            _dateTimeProvider = dateTimeProvider;
            _summaryService = summaryService;
            _userStatsService = userStatsService;
        }

        public async Task<List<DailyIntake>> GetDailyIntakeAsync(string userId, DateOnly date)
        {
            return await _context.DailyIntakes
                .Include(i => i.Product)
                .Where(i => i.UserId == userId && i.Date == date)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddIntakeAsync(string userId, int productId, decimal quantity, DateOnly date)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new Exception("Product not found.");

            var intake = new DailyIntake
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                Date = date
            };

            _context.DailyIntakes.Add(intake);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveIntakeAsync(int intakeId)
        {
            var intake = await _context.DailyIntakes.FindAsync(intakeId);
            if (intake != null)
            {
                _context.DailyIntakes.Remove(intake);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateIntakeQuantityAsync(int intakeId, decimal newQuantity)
        {
            var intake = await _context.DailyIntakes.FindAsync(intakeId);
            if (intake != null)
            {
                intake.Quantity = newQuantity;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<DailySummary> GetDailySummaryAsync(string userId, DateOnly date, int dailyGoal)
        {
            var intakes = await GetDailyIntakeAsync(userId, date);
            return _summaryService.Generate(intakes, date, dailyGoal);
        }

        public async Task<UserStats> GetUserStatsAsync(string userId, DateOnly startDate, DateOnly endDate)
        {
            var intakes = await _context.DailyIntakes
                .Include(i => i.Product)
                .Where(i => i.UserId == userId && i.Date >= startDate && i.Date <= endDate)
                .ToListAsync();

            return _userStatsService.Generate(intakes, startDate, endDate);
        }

        // Copies all intakes from sourceDate to targetDate for the given user.
        // Returns the number of items copied, or -1 if the target date already has intakes.
        public async Task<int> CopyDailyIntakeAsync(string userId, DateOnly sourceDate, DateOnly targetDate)
        {
            // load source intakes
            var sourceIntakes = await _context.DailyIntakes
                .Where(i => i.UserId == userId && i.Date == sourceDate)
                .ToListAsync();

            if (!sourceIntakes.Any()) return 0;

            // if target already has intakes, do not duplicate
            var targetExists = await _context.DailyIntakes
                .AnyAsync(i => i.UserId == userId && i.Date == targetDate);

            if (targetExists) return -1;

            var copies = sourceIntakes.Select(i => new DailyIntake
            {
                UserId = userId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Date = targetDate
            }).ToList();

            _context.DailyIntakes.AddRange(copies);
            await _context.SaveChangesAsync();

            return copies.Count;
        }
    }
}