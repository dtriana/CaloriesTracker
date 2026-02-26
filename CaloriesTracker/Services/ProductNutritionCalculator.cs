using CaloriesTracker.Models;
using CaloriesTracker.Services.Interfaces;

namespace CaloriesTracker.Services
{
    public class ProductNutritionCalculator : IProductNutritionCalculator
    {
        public decimal CalculateCalories(Food product, decimal quantity) =>
            product.CaloriesPerPortion * quantity / 100;

        public decimal CalculateProtein(Food product, decimal quantity) =>
            product.ProteinPerPortion * quantity / 100;

        public decimal CalculateFat(Food product, decimal quantity) =>
            product.FatPerPortion * quantity / 100;

        public decimal CalculateCarbs(Food product, decimal quantity) =>
            product.CarbsPerPortion * quantity / 100;
    }
}
