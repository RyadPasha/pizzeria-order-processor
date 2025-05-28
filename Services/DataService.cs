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
            EnsureFileExists(filePath, "Products");

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
            EnsureFileExists(filePath, "Ingredients");

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

        public List<Order> LoadOrders(string? filePath = null)
        {
            filePath ??= _config.OrdersFilePath;
            EnsureFileExists(filePath, "Orders");

            var extension = Path.GetExtension(filePath).ToLower();

            return extension switch
            {
                ".json" => ParseJsonOrders(filePath),
                ".csv" => ParseCsvOrders(filePath),
                _ => throw new ArgumentException($"Unsupported file format: {extension}. Use JSON or CSV.")
            };
        }

        private static void EnsureFileExists(string filePath, string fileDescription)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"{fileDescription} file not found: {filePath}");
            }
        }

        private static List<Order> ParseJsonOrders(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var orders = JsonSerializer.Deserialize<List<Order>>(json, options) ?? new List<Order>();
            Console.WriteLine($"Loaded {orders.Count} order entries from {filePath}");
            return orders;
        }

        private List<Order> ParseCsvOrders(string filePath)
        {
            var orders = new List<Order>();
            var lines = File.ReadAllLines(filePath);

            if (lines.Length == 0) return orders;

            var headers = lines[0].Split(',').Select(h => h.Trim().ToLower()).ToArray();
            var columnMap = CreateColumnMap(headers);

            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    var values = lines[i].Split(',');
                    var order = ParseCsvOrder(values, columnMap);
                    if (order != null) orders.Add(order);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing CSV line {i + 1}: {ex.Message}");
                }
            }

            Console.WriteLine($"Loaded {orders.Count} order entries from {filePath}");
            return orders;
        }

        private static Dictionary<string, int> CreateColumnMap(string[] headers)
        {
            var map = new Dictionary<string, int>();
            for (int i = 0; i < headers.Length; i++)
            {
                map[headers[i]] = i;
            }
            return map;
        }

        private Order ParseCsvOrder(string[] values, Dictionary<string, int> columnMap)
        {
            return new Order
            {
                OrderId = GetColumnValue(values, columnMap, "orderid"),
                ProductId = GetColumnValue(values, columnMap, "productid"),
                Quantity = int.Parse(GetColumnValue(values, columnMap, "quantity")),
                DeliveryAt = DateTime.Parse(GetColumnValue(values, columnMap, "deliveryat")),
                CreatedAt = DateTime.Parse(GetColumnValue(values, columnMap, "createdat")),
                DeliveryAddress = GetColumnValue(values, columnMap, "deliveryaddress")
            };
        }

        private static string GetColumnValue(string[] values, Dictionary<string, int> columnMap, string columnName)
        {
            return columnMap.TryGetValue(columnName, out int index) && index < values.Length
                ? values[index].Trim().Trim('"')
                : "";
        }
    }
}
