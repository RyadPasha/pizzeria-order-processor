/******************************************************************************
 * File:        AppConfig.cs
 * Author:      Mohamed Riyad
 * Created:     27 May, 2025
 * Description: Configuration settings for the Pizzeria Order Processor application.
 ******************************************************************************/

using System.IO;

namespace PizzeriaOrderProcessor.Configuration
{
    public class AppConfig
    {
        private readonly string DataDir = "Data";

        public string ProductsFilePath => Path.Combine(DataDir, "products.json");
        public string IngredientsFilePath => Path.Combine(DataDir, "ingredients.json");
        public string OrdersFilePath => Path.Combine(DataDir, "orders.json");
    }
}
