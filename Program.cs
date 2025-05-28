// -----------------------------------------------------------------------------
// File: Program.cs
// Project: PizzeriaOrderProcessor
// Author: Mohamed Riyad
// Created: 28 May, 2025
// Description: Entry point for the console application that reads order,
//              product, and ingredient data, processes the orders, and
//              outputs a summary report.
// -----------------------------------------------------------------------------

using System;
using PizzeriaOrderProcessor.Configuration;
using PizzeriaOrderProcessor.Services;

namespace PizzeriaOrderProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("\n\n╭━━━╮╱╱╱╱╱╱╱╱╱╭╮╱╱╱╱╭━━━╮\n┃╭━╮┃╱╱╱╱╱╱╱╱╱┃┣╮╱╱╱┃╭━╮┃\n┃╰━╯┣┳╮╱╭┳━━┳━╯┃┣━━╮┃┃╱┃┣╮╭┳━━┳━╮\n┃╭╮╭╋┫┃╱┃┃╭╮┃╭╮┣┫━━┫┃┃╱┃┃╰╯┃┃━┫╭╮╮\n┃┃┃╰┫┃╰━╯┃╭╮┃╰╯┃┣━━┃┃╰━╯┣╮╭┫┃━┫┃┃┃\n╰╯╰━┻┻━╮╭┻╯╰┻━━╯╰━━╯╰━━━╯╰╯╰━━┻╯╰╯\n╱╱╱╱╱╭━╯┃ Orders Processing System\n╱╱╱╱╱╰━━╯ By: Mohamed Riyad :)\n");
            Console.ResetColor();
            Console.WriteLine(new string('=', 35));

            try
            {
                var config = new AppConfig();

                // Setup services
                var dataService = new DataService(config);
                var orderProcessor = new OrderProcessor(dataService);

                // Process orders
                orderProcessor.ProcessOrders();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application error: {ex.Message}");
            }
        }
    }
}
