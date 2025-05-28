using System.Collections.Generic;
using PizzeriaOrderProcessor.Models;

namespace PizzeriaOrderProcessor.Services
{
    public interface IOrderProcessor
    {
        void ProcessOrders(string? orderFilePath = null);
        List<OrderSummary> ProcessValidOrders(List<Order> orders, out Dictionary<string, decimal> totalIngredients);
    }
}
