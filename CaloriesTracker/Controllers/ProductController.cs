using CaloriesTracker.Models;
using CaloriesTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CaloriesTracker.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly FoodService _productService;

        public ProductController(FoodService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string sortBy = "Name")
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var products = await _productService.GetUserProductsAsync(userId);

            products = sortBy switch
            {
                "Calories" => products.OrderByDescending(p => p.CaloriesPerPortion).ToList(),
                "Protein" => products.OrderByDescending(p => p.ProteinPerPortion).ToList(),
                _ => products.OrderBy(p => p.Name).ToList(),
            };

            ViewBag.SortBy = sortBy;
            return View(products);
        }


        [HttpPost]
        public async Task<IActionResult> Add(Food food)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();
            await _productService.AddFoodAsync(food, userId);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var product = await _productService.GetProductByIdAsync(id, userId);
            if (product == null) return NotFound();

            return View(product);
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var success = await _productService.DeleteProductAsync(id, userId);
            if (!success) return NotFound();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var product = await _productService.GetProductByIdAsync(id, userId);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Food product)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var culture = System.Globalization.CultureInfo.InvariantCulture;

            if (!ModelState.IsValid) return View(product);

            product.UserId = userId;
            var success = await _productService.UpdateProductAsync(product, userId);

            if (!success) return Unauthorized();

            return RedirectToAction("Index");
        }


    }
}
