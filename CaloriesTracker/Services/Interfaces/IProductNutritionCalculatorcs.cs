using CaloriesTracker.Models;

namespace CaloriesTracker.Services.Interfaces
{
    public interface IProductNutritionCalculator
    {
        decimal CalculateCalories(Food product, decimal quantity);
        decimal CalculateProtein(Food product, decimal quantity);
        decimal CalculateFat(Food product, decimal quantity);
        decimal CalculateCarbs(Food product, decimal quantity);
    }
}
