// -----------------------------------------------------------------------------
// File: Order.cs
// Project: PizzeriaOrderProcessor
// Author: Mohamed Riyad <m@ryad.dev>
// Created: 26 May, 2025
// Description: Represents a customer order in the system.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace PizzeriaOrderProcessor.Models
{
    public class Order
    {
        public required string OrderId { get; set; }
        public required string ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime DeliveryAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string DeliveryAddress { get; set; }
    }

    public class Product
    {
        public required string ProductName { get; set; }
        public decimal Price { get; set; }
    }

    public class Ingredient
    {
        public required string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class ProductIngredients
    {
        public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();
    }

    public class OrderItem
    {
        public required string ProductId { get; set; }
        public required string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }

    // Aggregated order information
    public class OrderSummary
    {
        public required string OrderId { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal TotalPrice { get; set; }
        public DateTime DeliveryAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string DeliveryAddress { get; set; }
    }
}
