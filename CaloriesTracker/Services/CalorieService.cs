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

    public class CalorieService(
        AppDbContext context,
        IDailySummaryService summaryService,
        IUserStatsService userStatsService)
    {
        public async Task<List<DailyIntake>> GetDailyIntakeAsync(string userId, DateOnly date)
        {
            return await context.DailyIntakes
                .Include(i => i.Product)
                .Where(i => i.UserId == userId && i.Date == date)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddIntakeAsync(string userId, int productId, decimal quantity, DateOnly date)
        {
            var product = await context.Products.FindAsync(productId);
            if (product == null) throw new Exception("Product not found.");

            var intake = new DailyIntake
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity,
                Date = date
            };

            context.DailyIntakes.Add(intake);
            await context.SaveChangesAsync();
        }

        public async Task RemoveIntakeAsync(int intakeId)
        {
            var intake = await context.DailyIntakes.FindAsync(intakeId);
            if (intake != null)
            {
                context.DailyIntakes.Remove(intake);
                await context.SaveChangesAsync();
            }
        }

        public async Task UpdateIntakeQuantityAsync(int intakeId, decimal newQuantity)
        {
            var intake = await context.DailyIntakes.FindAsync(intakeId);
            if (intake != null)
            {
                intake.Quantity = newQuantity;
                await context.SaveChangesAsync();
            }
        }

        public async Task<DailySummary> GetDailySummaryAsync(string userId, DateOnly date, int dailyGoal)
        {
            var intakes = await GetDailyIntakeAsync(userId, date);
            var weigth = await context.DailyWeight
                .Where(d => d.Date == date && d.UserId == userId)
                .Select(u => u.WeightInKg).FirstOrDefaultAsync();
            return summaryService.Generate(intakes, date, weigth);
        }

        public async Task<UserStats> GetUserStatsAsync(string userId, DateOnly startDate, DateOnly endDate)
        {
            var intakes = await context.DailyIntakes
                .Include(i => i.Product)
                .Where(i => i.UserId == userId && i.Date >= startDate && i.Date <= endDate)
                .ToListAsync();

            return userStatsService.Generate(intakes, startDate, endDate);
        }

        // Copies all intakes from sourceDate to targetDate for the given user.
        // Returns the number of items copied, or -1 if the target date already has intakes.
        public async Task<int> CopyDailyIntakeAsync(string userId, DateOnly sourceDate, DateOnly targetDate)
        {
            // load source intakes
            var sourceIntakes = await context.DailyIntakes
                .Where(i => i.UserId == userId && i.Date == sourceDate)
                .ToListAsync();

            if (!sourceIntakes.Any()) return 0;

            // if target already has intakes, do not duplicate
            var targetExists = await context.DailyIntakes
                .AnyAsync(i => i.UserId == userId && i.Date == targetDate);

            if (targetExists) return -1;

            var copies = sourceIntakes.Select(i => new DailyIntake
            {
                UserId = userId,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Date = targetDate
            }).ToList();

            context.DailyIntakes.AddRange(copies);
            await context.SaveChangesAsync();

            return copies.Count;
        }
    }
}