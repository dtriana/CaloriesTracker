using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CaloriesTracker.Models;             
using CaloriesTracker.Models.ViewModels;

namespace CaloriesTracker.Services
{
    public class ReportService
    {
        private readonly CalorieService _calorieService;

        public ReportService(CalorieService calorieService)
        {
            _calorieService = calorieService;
        }

        public async Task<DailyReportViewModel> GetDailyReportAsync(string userId, DateOnly date)
        {
            var summary = await _calorieService.GetDailySummaryAsync(userId, date,0);

            var intakes = summary.Intakes ?? new List<DailyIntake>();

            var vm = new DailyReportViewModel
            {
                Date = date,
                TotalCalories = (double)summary.TotalCalories,
                DailyGoal = (double)summary.DailyGoal,
                TotalProtein = (double)summary.TotalProtein,
                TotalFat = (double)summary.TotalFat,
                TotalCarbs = (double)summary.TotalCarbs,

                ConsumedProducts = intakes
                    .Select(i =>
                    {
                      
                        if (i.Product == null) return null;

                        double qty = (double)i.Quantity;
                        double cal100 = (double)i.Product.CaloriesPerPortion;
                        double prot100 = (double)i.Product.ProteinPerPortion;
                        double fat100 = (double)i.Product.FatPerPortion;
                        double carb100 = (double)i.Product.CarbsPerPortion;

                        return new ConsumedProductViewModel
                        {
                            ProductName = i.Product.Name,
                            Quantity = qty,
                            Calories = cal100 * qty / 100.0,
                            Protein = prot100 * qty / 100.0,
                            Fat = fat100 * qty / 100.0,
                            Carbs = carb100 * qty / 100.0
                        };
                    })
                    .Where(x => x != null)!
                    .ToList()!
            };

            return vm;
        }
    }
}
