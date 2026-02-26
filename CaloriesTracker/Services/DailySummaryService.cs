using CaloriesTracker.Models.Stats;
using CaloriesTracker.Models;
using CaloriesTracker.Services.Interfaces;

namespace CaloriesTracker.Services
{
    public class DailySummaryService(IProductNutritionCalculator calculator) : IDailySummaryService
    {
        public DailySummary Generate(List<DailyIntake> intakes, DateOnly date, int dailyGoal)
        {
            var summary = new DailySummary
            {
                Date = date,
                TotalCalories = intakes.Sum(i => calculator.CalculateCalories(i.Product, i.Quantity)),
                TotalProtein = intakes.Sum(i => calculator.CalculateProtein(i.Product, i.Quantity)),
                TotalFat = intakes.Sum(i => calculator.CalculateFat(i.Product, i.Quantity)),
                TotalCarbs = intakes.Sum(i => calculator.CalculateCarbs(i.Product, i.Quantity)),
                DailyGoal = dailyGoal,
                Intakes = intakes
            };
            return summary;
        }
    }

}
