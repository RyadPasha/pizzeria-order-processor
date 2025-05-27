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
    }
}
