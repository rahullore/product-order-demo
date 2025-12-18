

namespace CustomerOrders.Api.Data.Entities;

public sealed class Order
{
   public int Id { get; set; }
   public int ProductId { get; set; }
   public Product Product { get; set; } = null!;

   public int Quantity { get; set; }
   public decimal UnitPrice { get; set; }

   public DateTime CreatedAtUtc { get; set; }= DateTime.UtcNow;
}