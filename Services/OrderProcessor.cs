// -----------------------------------------------------------------------------
// File: OrderProcessor.cs
// Author: Mohamed Riyad
// Created: 27 May, 2025
// Description: This file contains the OrderProcessor class, which is responsible for
// processing pizza orders, including validation, pricing, and inventory management.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using PizzeriaOrderProcessor.Models;

namespace PizzeriaOrderProcessor.Services
{
    /// <summary>
    /// Processes customer orders by validating, aggregating, and summarizing data.
    /// </summary>
    public class OrderProcessor : IOrderProcessor
    {
        private readonly IDataService _dataService;
        private readonly IOrderValidator _validator;
        private readonly Dictionary<string, Product> _products;
        private readonly Dictionary<string, ProductIngredients> _productIngredients;

        /// <summary>
        /// Initializes the OrderProcessor with loaded reference data.
        /// </summary>
        /// <param name="dataService">Injected data service for loading products, ingredients, and orders.</param>
        public OrderProcessor(IDataService dataService)
        {
            _dataService = dataService;
            _products = _dataService.LoadProducts();
            _productIngredients = _dataService.LoadProductIngredients();
            _validator = new OrderValidator(_products);
        }

        /// <summary>
        /// Processes and validates a list of orders, returning valid order summaries and total ingredients required.
        /// </summary>
        /// <param name="orders">List of raw orders.</param>
        /// <param name="totalIngredients">Output dictionary of aggregated ingredient requirements.</param>
        /// <returns>List of validated and summarized orders.</returns>
        public List<OrderSummary> ProcessValidOrders(List<Order> orders, out Dictionary<string, decimal> totalIngredients)
        {
            totalIngredients = new Dictionary<string, decimal>();

            // Group orders by OrderId for aggregation
            var groupedOrders = GroupOrdersByOrderId(orders);
            Console.WriteLine($"Found {groupedOrders.Count} unique orders from {orders.Count} entries");

            var validOrderSummaries = new List<OrderSummary>();

            foreach (var orderGroup in groupedOrders.Values)
            {
                if (_validator.IsOrderGroupValid(orderGroup, out var errors))
                {
                    var summary = CreateOrderSummary(orderGroup);
                    validOrderSummaries.Add(summary);

                    // Accumulate ingredients
                    AccumulateIngredients(orderGroup, totalIngredients);
                }
                else
                {
                    // Log validation errors
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Validation Error: {error}");
                    }
                }
            }

            return validOrderSummaries;
        }

        /// <summary>
        /// Groups a list of orders by their OrderId.
        /// </summary>
        /// <param name="orders">List of orders.</param>
        /// <returns>Dictionary of grouped orders by OrderId.</returns>
        private Dictionary<string, List<Order>> GroupOrdersByOrderId(List<Order> orders)
        {
            // Group orders by OrderId
            var grouped = new Dictionary<string, List<Order>>();

            foreach (var order in orders)
            {
                if (!grouped.ContainsKey(order.OrderId))
                    grouped[order.OrderId] = new List<Order>();

                grouped[order.OrderId].Add(order);
            }

            return grouped;
        }

        /// <summary>
        /// Creates a summarized representation of an order group including pricing and items.
        /// </summary>
        /// <param name="orderGroup">Grouped orders with the same OrderId.</param>
        /// <returns>Summarized order details.</returns>
        private OrderSummary CreateOrderSummary(List<Order> orderGroup)
        {
            var first = orderGroup.First();
            var summary = new OrderSummary
            {
                OrderId = first.OrderId,
                DeliveryAt = first.DeliveryAt,
                CreatedAt = first.CreatedAt,
                DeliveryAddress = first.DeliveryAddress
            };

            // Aggregate quantities by product
            var productQuantities = new Dictionary<string, int>();
            foreach (var order in orderGroup)
            {
                productQuantities[order.ProductId] = productQuantities.GetValueOrDefault(order.ProductId, 0) + order.Quantity;
            }

            // Create order items with pricing
            foreach (var kvp in productQuantities)
            {
                if (_products.TryGetValue(kvp.Key, out var product))
                {
                    var item = new OrderItem
                    {
                        ProductId = kvp.Key,
                        ProductName = product.ProductName,
                        Quantity = kvp.Value,
                        UnitPrice = product.Price,
                        TotalPrice = product.Price * kvp.Value
                    };
                    summary.Items.Add(item);
                    summary.TotalPrice += item.TotalPrice;
                }
            }

            return summary;
        }

        /// <summary>
        /// Aggregates the total amount of ingredients needed for a group of orders.
        /// </summary>
        /// <param name="orderGroup">Grouped orders to calculate ingredient totals from.</param>
        /// <param name="totalIngredients">Dictionary to accumulate ingredient quantities.</param>
        private void AccumulateIngredients(List<Order> orderGroup, Dictionary<string, decimal> totalIngredients)
        {
            // Aggregate product quantities first
            var productQuantities = new Dictionary<string, int>();
            foreach (var order in orderGroup)
            {
                productQuantities[order.ProductId] = productQuantities.GetValueOrDefault(order.ProductId, 0) + order.Quantity;
            }

            // Calculate ingredient requirements
            foreach (var kvp in productQuantities)
            {
                if (_productIngredients.TryGetValue(kvp.Key, out var productIngredients))
                {
                    foreach (var ingredient in productIngredients.Ingredients)
                    {
                        var totalAmount = ingredient.Amount * kvp.Value;
                        totalIngredients[ingredient.Name] = totalIngredients.GetValueOrDefault(ingredient.Name, 0) + totalAmount;
                    }
                }
            }
        }

        /// <summary>
        /// Displays processed order summaries and the required ingredients to the console.
        /// </summary>
        /// <param name="validOrders">List of validated and processed orders.</param>
        /// <param name="totalIngredients">Aggregated ingredient requirements.</param>
        private static void DisplayResults(List<OrderSummary> validOrders, Dictionary<string, decimal> totalIngredients)
        {
            Console.WriteLine("\n" + new string('=', 35));
            Console.WriteLine("ORDER PROCESSING SUMMARY");
            Console.WriteLine(new string('=', 35));

            Console.WriteLine($"\nProcessed {validOrders.Count} valid orders:");
            Console.WriteLine(new string('-', 35));

            foreach (var order in validOrders.OrderBy(o => o.OrderId))
            {
                Console.WriteLine($"\nOrder ID: {order.OrderId}");
                Console.WriteLine($"Created: {order.CreatedAt:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"Delivery: {order.DeliveryAt:yyyy-MM-dd HH:mm}");
                Console.WriteLine($"Address: {order.DeliveryAddress}");
                Console.WriteLine("Items:");

                foreach (var item in order.Items)
                {
                    Console.WriteLine($"  - {item.ProductName} x{item.Quantity} @ AED {item.UnitPrice:F2} = AED {item.TotalPrice:F2}");
                }
                Console.WriteLine($"Total: AED {order.TotalPrice:F2}");
            }

            Console.WriteLine($"\nGrand Total: AED {validOrders.Sum(o => o.TotalPrice):F2}");

            Console.WriteLine("\n" + new string('-', 35));
            Console.WriteLine("INGREDIENT REQUIREMENTS");
            Console.WriteLine(new string('-', 35));

            foreach (var ingredient in totalIngredients.OrderBy(kvp => kvp.Key))
            {
                Console.WriteLine($"{ingredient.Key}: {ingredient.Value:F2} units");
            }
        }

        /// <summary>
        /// Loads and processes customer orders, then prints order summaries and ingredient needs.
        /// </summary>
        /// <param name="orderFilePath">Optional path to the order file. Uses default if not provided.</param>
        public void ProcessOrders(string? orderFilePath = null)
        {
            try
            {
                // Load orders using default path if none provided
                var orders = _dataService.LoadOrders(orderFilePath);

                // Process orders and get results
                var validOrderSummaries = ProcessValidOrders(orders, out var totalIngredients);

                // Display results
                DisplayResults(validOrderSummaries, totalIngredients);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing orders: {ex.Message}");
            }
        }
    }
}
