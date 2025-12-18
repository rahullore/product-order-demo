using CustomerOrders.Api.Models;
using Microsoft.VisualBasic;

namespace CustomerOrders.Api.Services;

public interface IInMemoryStore
{
    IReadOnlyList<ProductDto> GetProducts();
    ProductDto? GetProduct(int id);
    IReadOnlyList<OrderDto> GetOrders();
    OrderDto? GetOrder(int id);
    OrderDto AddOrder(int productId, int quantity);
    void ClearOrders();
    ProductDto AddProduct(string name, string description, decimal price, int stock);
}

public sealed class InMemoryStore : IInMemoryStore
{
    private readonly List<ProductDto> _products;
    private readonly List<OrderDto> _orders = new();
    private int _nextOrderId = 1;

    public InMemoryStore()
    {
        // Seed some products (similar to your Nuxt dummy data)
        _products = new List<ProductDto>
        {
            new(1, "Basic Monitor", "A simple medical monitor", 199.99m, 10),
            new(2, "Advanced Monitor", "Advanced patient monitor with more features", 499.99m, 5),
            new(3, "ECG Cable", "Standard ECG cable", 39.99m, 25),
            new(4, "Blood Pressure Cuff", "Automatic blood pressure cuff", 59.99m, 15),
            new(5, "Pulse Oximeter", "Finger pulse oximeter",29.99m, 30),
            new(6, "Thermometer", "Digital thermometer", 19.99m, 50),
            new(7, "Infusion Pump", "IV infusion pump", 899.99m, 3),
            new(8, "Syringe Pump", "Precision syringe pump", 799.99m, 4),
            new(9, "Ventilator", "Mechanical ventilator", 4999.99m, 2),
            new(10, "Defibrillator", "Portable defibrillator", 2999.99m, 1),
            new(11, "ECG Machine", "12-lead ECG machine", 1499.99m, 6),
            new(12, "Ultrasound Machine", "Portable ultrasound machine", 3999.99m, 2),
            new(13, "X-Ray Machine", "Digital X-ray machine", 9999.99m, 1),
            new(14, "MRI Scanner", "High-field MRI scanner", 49999.99m, 1),
            new(15, "CT Scanner", "Multi-slice CT scanner", 29999.99m, 1),
            new(16, "Surgical Light", "LED surgical light", 1499.99m, 8),
            new(17, "Anesthesia Machine", "Advanced anesthesia machine", 7999.99m, 2),
            new(18, "Patient Monitor", "Multi-parameter patient monitor", 2499.99m, 5),
            new(19, "Infusion Stand", "Adjustable infusion stand", 199.99m, 20),
            new(20, "Medical Cart", "Mobile medical equipment cart", 299.99m, 10)
        };
    }

    public IReadOnlyList<ProductDto> GetProducts() => _products;

    public ProductDto? GetProduct(int id) =>
        _products.FirstOrDefault(p => p.Id == id);

    public IReadOnlyList<OrderDto> GetOrders() => _orders;

    public OrderDto? GetOrder(int id) =>
        _orders.FirstOrDefault(o => o.Id == id);

    public OrderDto AddOrder(int productId, int quantity)
    {
        var product = GetProduct(productId)
            ?? throw new InvalidOperationException($"Product {productId} not found.");

        var order = new OrderDto(
            Id: _nextOrderId++,
            ProductId: product.Id,
            ProductName: product.Name,
            Quantity: quantity,
            Price: product.Price,
            CreatedAtUtc: DateTime.UtcNow
        );

        _orders.Add(order);
        return order;
    }

    public void ClearOrders()
    {
        _orders.Clear();
        _nextOrderId = 1;
    }

    public ProductDto AddProduct(string name, string description, decimal price, int stock)
    {
       if(string.IsNullOrWhiteSpace(name))
       {
            throw new ArgumentException("Product name cannot be empty.", nameof(name));
       }
         if(price < 0)
         {
                throw new ArgumentException("Product price cannot be negative.", nameof(price));
         }
         if(stock < 0)
         {
                throw new ArgumentException("Product stock cannot be negative.", nameof(stock));
         }
         var product = new ProductDto(
            Id: _products.Max(p => p.Id) + 1,
            Name: name,
            Description: description,
            Price: price,
            Stock: stock
         );
         _products.Add(product);
         return product;
    }
}
