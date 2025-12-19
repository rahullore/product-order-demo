using CustomerOrders.Api.Models;
using CustomerOrders.Api.Data;
using CustomerOrders.Api.Data.Entities;

namespace CustomerOrders.Api.Services;

public sealed class ProductOrderDbService: IInMemoryStore
{
    private readonly AppDbContext _appDbContext;

    public ProductOrderDbService(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
        
    }

    public IReadOnlyList<ProductDto> GetProducts()
    {
        return _appDbContext.Products
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Stock))
            .ToList();
    }

    public ProductDto? GetProduct(int id) =>
        _appDbContext.Products
            .Where(p => p.Id == id)
            .Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.Stock))
            .FirstOrDefault();

    public IReadOnlyList<OrderDto> GetOrders()
    {
        return _appDbContext.Orders
            .Select(o => new OrderDto(
                o.Id,
                o.ProductId,        
                o.Product.Name,
                o.Quantity,
                o.UnitPrice,
                o.CreatedAtUtc))
            .ToList();  
    }
    public OrderDto? GetOrder(int id) =>
        _appDbContext.Orders
            .Where(o => o.Id == id)
            .Select(o => new OrderDto(
                o.Id,
                o.ProductId,
                o.Product.Name,
                o.Quantity,
                o.UnitPrice,
                o.CreatedAtUtc))
            .FirstOrDefault();

    public OrderDto AddOrder(int productId, int quantity)
    {
        var product = _appDbContext.Products
            .FirstOrDefault(p => p.Id == productId)
            ?? throw new InvalidOperationException($"Product {productId} not found.");
        
        var order = new OrderDto(
            Id: 0,
            ProductId: product.Id,
            ProductName: product.Name,
            Quantity: quantity,
            Price: product.Price,
            CreatedAtUtc: DateTime.UtcNow
        );
        var orderEntity = new Order
        {
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            UnitPrice = order.Price,
            CreatedAtUtc = order.CreatedAtUtc
        };
        _appDbContext.Orders.Add(orderEntity);
        _appDbContext.SaveChanges();    
        return new OrderDto(
            Id: orderEntity.Id,
            ProductId: orderEntity.ProductId,
            ProductName: product.Name,
            Quantity: orderEntity.Quantity,
            Price: orderEntity.UnitPrice,
            CreatedAtUtc: orderEntity.CreatedAtUtc
        );
    }
    public void ClearOrders()
    {
        var allOrders = _appDbContext.Orders.ToList();
        _appDbContext.Orders.RemoveRange(allOrders);
        _appDbContext.SaveChanges();
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
         var productEntity = new Product
         {
            Name = name,
            Description = description,
            Price = price,
            Stock = stock
         };
         _appDbContext.Products.Add(productEntity);
         _appDbContext.SaveChanges();
         return new ProductDto(
            Id: productEntity.Id,
            Name: productEntity.Name,
            Description: productEntity.Description,
            Price: productEntity.Price,
            Stock: productEntity.Stock
         );
    }
}

    
