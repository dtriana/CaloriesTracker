using CaloriesTracker.Models.Stats;
using CaloriesTracker.Models;
using CaloriesTracker.Services.Interfaces;

namespace CaloriesTracker.Services
{
    public class DailySummaryService(IProductNutritionCalculator calculator) : IDailySummaryService
    {
        private static readonly decimal KgToPounds = 2.20462m;
        private static readonly decimal TenCalsPerPoundMinusTenPercent = 9;
        public DailySummary Generate(List<DailyIntake> intakes, DateOnly date, decimal weigth)
        {
            var summary = new DailySummary
            {
                Date = date,
                TotalCalories = intakes.Sum(i => calculator.CalculateCalories(i.Product, i.Quantity)),
                TotalProtein = intakes.Sum(i => calculator.CalculateProtein(i.Product, i.Quantity)),
                TotalFat = intakes.Sum(i => calculator.CalculateFat(i.Product, i.Quantity)),
                TotalCarbs = intakes.Sum(i => calculator.CalculateCarbs(i.Product, i.Quantity)),
                DailyGoal = weigth * KgToPounds * TenCalsPerPoundMinusTenPercent,
                ProteinGoal = weigth * KgToPounds,
                Intakes = intakes
            };
            return summary;
        }
    }

}
