using CaloriesTracker.Models;
using CaloriesTracker.Services.Interfaces;

namespace CaloriesTracker.Services
{
    public class ProductNutritionCalculator : IProductNutritionCalculator
    {
        public decimal CalculateCalories(Food product, decimal quantity) =>
            (quantity / product.PortionSize) * product.CaloriesPerPortion;

        public decimal CalculateProtein(Food product, decimal quantity) =>
            (quantity / product.PortionSize) * product.ProteinPerPortion;

        public decimal CalculateFat(Food product, decimal quantity) =>
            (quantity / product.PortionSize) * product.FatPerPortion;

        public decimal CalculateCarbs(Food product, decimal quantity) =>
            (quantity / product.PortionSize) * product.CarbsPerPortion;
    }
}
