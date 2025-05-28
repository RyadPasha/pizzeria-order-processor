# Pizzeria Order Processing System (Riyad's Oven)

A .NET console application that processes customer orders for a pizzeria. It supports reading from JSON and CSV order files, validates entries, calculates total prices, and determines the required amount of ingredients based on a predefined product and ingredient list.

## Features

- Parses orders from both **JSON** and **CSV** files
- Validates each order entry and full order group
- Calculates **total price per order**
- Aggregates the **total amount of ingredients** needed
- Displays a clean **summary report** in the console

## Project Structure

```
PizzeriaOrderProcessor/
├── Configuration/
│   └── AppConfig.cs            # File path configuration
├── Models/
│   └── Order.cs                # Domain models for orders, products, ingredients, summaries
├── Services/
│   ├── DataService.cs          # Data loading from JSON/CSV
│   ├── OrderValidator.cs       # Business rules and validation logic
│   └── OrderProcessor.cs       # Core processing, aggregation, and reporting
├── Program.cs                  # Main entry point
└── Data/
    ├── products.json           # Sample product data
    ├── ingredients.json        # Sample ingredients data
    └── orders.json             # Sample orders (or use CSV)
```

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) or later

## How to Run

1. **Clone the Repository**
   ```bash
   git clone https://github.com/RyadPasha/pizzeria-order-processor.git
   cd pizzeria-order-processor
   ```

2. **Update the data files**
   Update `products.json`, `ingredients.json`, and `orders.json` (or `.csv`) inside the `Data/` directory if you wanted.

3. **Build and Run**
   ```bash
   dotnet run
   ```

   You'll see a console summary like:
   ```
   ╭━━━╮╱╱╱╱╱╱╱╱╱╭╮╱╱╱╱╭━━━╮
   ┃╭━╮┃╱╱╱╱╱╱╱╱╱┃┣╮╱╱╱┃╭━╮┃
   ┃╰━╯┣┳╮╱╭┳━━┳━╯┃┣━━╮┃┃╱┃┣╮╭┳━━┳━╮
   ┃╭╮╭╋┫┃╱┃┃╭╮┃╭╮┣┫━━┫┃┃╱┃┃╰╯┃┃━┫╭╮╮
   ┃┃┃╰┫┃╰━╯┃╭╮┃╰╯┃┣━━┃┃╰━╯┣╮╭┫┃━┫┃┃┃
   ╰╯╰━┻┻━╮╭┻╯╰┻━━╯╰━━╯╰━━━╯╰╯╰━━┻╯╰╯
   ╱╱╱╱╱╭━╯┃ Orders Processing System
   ╱╱╱╱╱╰━━╯ By: Mohamed Riyad :)

   ========================
   ORDER PROCESSING SUMMARY
   ========================
    Order ID: ORD001
    Created: 2025-05-23 16:00
    Delivery: 2025-05-23 18:30
    Address: Villa 12, Al Wasl Road, Dubai, UAE
    Items:
    - Margherita Pizza x2 @ AED 28.00 = AED 56.00
    - Garlic Bread x1 @ AED 14.00 = AED 14.00
    Total: AED 70.00

   Grand Total: AED 70.00

   -----------------------
   INGREDIENT REQUIREMENTS
   -----------------------
   Mozzarella Cheese: 2.20 units
   Tomato Sauce: 3.50 units
   ...
   ```

## Validation Rules

- `OrderId`, `ProductId`, and `DeliveryAddress` must not be empty
- `Quantity` must be positive
- `DeliveryAt` must be **after** `CreatedAt`
- `CreatedAt` must not be in the future
- `ProductId` must exist in the product catalog
- Orders with the same `OrderId` must have consistent delivery info

## Supported File Formats

- **JSON** (default structure: array of orders)
- **CSV** (must include headers: `OrderId,ProductId,Quantity,DeliveryAt,CreatedAt,DeliveryAddress`)

## Configuration

Paths to input files are set in `AppConfig.cs`. By default:
```csharp
Data/products.json
Data/ingredients.json
Data/orders.json
```

To use a custom order file, pass the file path as a command-line argument:
```bash
dotnet run -- "Data/orders_2.json"
```

## Sample Data (example `products.json`)
```json
{
  "P001": { "ProductName": "Margherita Pizza", "Price": 28.00 },
  "P002": { "ProductName": "Pepperoni Pizza", "Price": 34.00 }
}
```
> **Note:** The product ID is used as the key to allow constant-time (O(1)) access to product details.

## Author

- Mohamed Riyad <mohamed@ryad.dev>
- Created: 26 May 2025
