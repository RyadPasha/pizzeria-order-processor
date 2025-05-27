using System.Collections.Generic;
using PizzeriaOrderProcessor.Models;

namespace PizzeriaOrderProcessor.Services
{
    public interface IDataService
    {
        Dictionary<string, Product> LoadProducts();
        Dictionary<string, ProductIngredients> LoadProductIngredients();
        List<Order> LoadOrders(string? filePath = null);
    }
}
