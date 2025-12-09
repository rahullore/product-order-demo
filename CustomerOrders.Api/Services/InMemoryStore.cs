using CustomerOrders.Api.Models;

namespace CustomerOrders.Api.Services;

public interface IInMemoryStore
{
    IReadOnlyList<ProductDto> GetProducts();
    ProductDto? GetProduct(int id);

    IReadOnlyList<OrderDto> GetOrders();
    OrderDto? GetOrder(int id);
    OrderDto AddOrder(int productId, int quantity);
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
}
