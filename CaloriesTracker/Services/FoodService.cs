using CaloriesTracker.Data;
using CaloriesTracker.Models;
using Microsoft.EntityFrameworkCore; 
using System.Diagnostics.CodeAnalysis;

namespace CaloriesTracker.Services
{
    public class FoodService(AppDbContext context)
    {
        private readonly AppDbContext _context = context;

        public async Task<Food> AddFoodAsync(Food food, string userId)
        {
            food.UserId = userId;
            food.User = null;

            _context.Products.Add(food);
            await _context.SaveChangesAsync();
            return food;
        }

        public async Task<bool> DeleteProductAsync(int productId, string userId)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.UserId == userId);

            if (product == null) return false;

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Food>> GetUserProductsAsync(string userId)
        {
            return await _context.Products
                .Where(p => p.UserId == userId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Food?> GetProductByIdAsync(int productId, string userId)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId && p.UserId == userId);
        }

        public async Task<bool> UpdateProductAsync(Food updatedProduct, string userId)
        {
            var existingProduct = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == updatedProduct.Id && p.UserId == userId);

            if (existingProduct == null) return false;

            existingProduct.Name = updatedProduct.Name;
            existingProduct.CaloriesPerPortion = updatedProduct.CaloriesPerPortion;
            existingProduct.ProteinPerPortion = updatedProduct.ProteinPerPortion;
            existingProduct.FatPerPortion = updatedProduct.FatPerPortion;
            existingProduct.CarbsPerPortion = updatedProduct.CarbsPerPortion;
            existingProduct.PortionName = updatedProduct.PortionName;
            existingProduct.PortionSize = updatedProduct.PortionSize;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
