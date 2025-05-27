using System.Collections.Generic;
using PizzeriaOrderProcessor.Models;

namespace PizzeriaOrderProcessor.Services
{
    public interface IOrderProcessor
    {
        List<OrderSummary> ProcessValidOrders(List<Order> orders, out Dictionary<string, decimal> totalIngredients);
    }
}
