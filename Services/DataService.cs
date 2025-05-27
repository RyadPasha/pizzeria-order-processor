// -----------------------------------------------------------------------------
// File: DataService.cs
// Project: PizzeriaOrderProcessor
// Author: Mohamed Riyad
// Created: 27 May, 2025
// Description: Data service for loading and managing product and order data.
// -----------------------------------------------------------------------------

using System.Text.Json;
using PizzeriaOrderProcessor.Models;
using PizzeriaOrderProcessor.Configuration;

namespace PizzeriaOrderProcessor.Services
{
    public class DataService : IDataService
    {
        private readonly AppConfig _config;

        public DataService(AppConfig config)
        {
            _config = config;
        }

        public Dictionary<string, Product> LoadProducts()
        {
            var filePath = _config.ProductsFilePath;

            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // JSON structure optimized for O(1) access: { "ProductId": { ProductData } }
            var productDict = JsonSerializer.Deserialize<Dictionary<string, Product>>(json, options);

            if (productDict == null || productDict.Count == 0)
            {
                throw new InvalidDataException("No products found in file.");
            }

            Console.WriteLine($"Loaded {productDict.Count} products from {filePath}");
            return productDict;
        }

        public Dictionary<string, ProductIngredients> LoadProductIngredients()
        {
            var filePath = _config.IngredientsFilePath;

            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var ingredientDict = JsonSerializer.Deserialize<Dictionary<string, ProductIngredients>>(json, options);

            if (ingredientDict == null || ingredientDict.Count == 0)
            {
                throw new InvalidDataException("No ingredients found in file.");
            }

            Console.WriteLine($"Loaded ingredients for {ingredientDict.Count} products from {filePath}");
            return ingredientDict;
        }
    }
}
