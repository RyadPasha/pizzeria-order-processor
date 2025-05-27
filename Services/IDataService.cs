using System.Collections.Generic;
using PizzeriaOrderProcessor.Models;

namespace PizzeriaOrderProcessor.Services
{
    public interface IDataService
    {
        Dictionary<string, Product> LoadProducts();
        Dictionary<string, ProductIngredients> LoadProductIngredients();
    }
}
