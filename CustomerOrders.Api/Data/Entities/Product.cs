namespace CustomerOrders.Api.Data.Entities;

public sealed class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public List<Order> Orders { get; set; } 
}