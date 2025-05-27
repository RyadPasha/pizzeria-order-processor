// -----------------------------------------------------------------------------
// File: OrderValidator.cs
// Author: Mohamed Riyad
// Created: 27 May, 2025
// Description: This file contains the OrderValidator class, which is responsible for
// validating individual pizza orders and groups of orders. It checks for required fields,
// business logic rules, and product existence in the provided product list.
// -----------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System;
using PizzeriaOrderProcessor.Models;

namespace PizzeriaOrderProcessor.Services
{
    /// <summary>
    /// Validates individual orders and groups of orders according to defined business rules.
    /// </summary>
    public class OrderValidator : IOrderValidator
    {
        private readonly Dictionary<string, Product> _products;

        public OrderValidator(Dictionary<string, Product> products)
        {
            _products = products;
        }

        /// <summary>
        /// Validates a single order for correctness and business rule compliance.
        /// </summary>
        /// <param name="order">The order to validate.</param>
        /// <param name="errors">A list that will contain validation error messages if any are found.</param>
        /// <returns><c>true</c> if the order is valid; otherwise, <c>false</c>.</returns>
        public bool IsValid(Order order, out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrWhiteSpace(order.OrderId))
                errors.Add("OrderId cannot be empty");

            if (string.IsNullOrWhiteSpace(order.ProductId))
                errors.Add("ProductId cannot be empty");

            if (order.Quantity <= 0)
                errors.Add("Quantity must be positive");

            if (string.IsNullOrWhiteSpace(order.DeliveryAddress))
                errors.Add("DeliveryAddress cannot be empty");

            // Product existence check - O(1) check
            if (!string.IsNullOrWhiteSpace(order.ProductId) && !_products.ContainsKey(order.ProductId))
                errors.Add($"Product {order.ProductId} does not exist");

            return errors.Count == 0;
        }

        /// <summary>
        /// Validates a group of orders for individual correctness and consistency across the group.
        /// </summary>
        /// <param name="orderGroup">The list of orders to validate as a group.</param>
        /// <param name="errors">A list that will contain validation error messages if any are found.</param>
        /// <returns><c>true</c> if all orders are valid and consistent; otherwise, <c>false</c>.</returns>
        public bool IsOrderGroupValid(List<Order> orderGroup, out List<string> errors)
        {
            errors = new List<string>();
            var hasIndividualErrors = false;

            // Validate each order individually
            foreach (var order in orderGroup)
            {
                if (!IsValid(order, out var individualErrors))
                {
                    errors.AddRange(individualErrors.Select(e => $"Order {order.OrderId}: {e}"));
                    hasIndividualErrors = true;
                }
            }

            // Validate consistency within order group
            if (!hasIndividualErrors && orderGroup.Count > 1)
            {
                var first = orderGroup.First();
                var inconsistentOrders = orderGroup.Where(o =>
                    o.DeliveryAt != first.DeliveryAt ||
                    o.CreatedAt != first.CreatedAt ||
                    o.DeliveryAddress != first.DeliveryAddress).ToList();

                if (inconsistentOrders.Any())
                {
                    errors.Add($"Order {first.OrderId} has inconsistent delivery details across entries");
                }
            }

            return errors.Count == 0;
        }
    }
}
