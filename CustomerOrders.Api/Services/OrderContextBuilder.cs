using System.Text;
using CustomerOrders.Api.Models;

namespace CustomerOrders.Api.Services;

public interface IOrderContextBuilder
{
    string BuildOrderContext(IReadOnlyList<OrderDto> orders);
}

public sealed class OrderContextBuilder : IOrderContextBuilder
{
    public string BuildOrderContext(IReadOnlyList<OrderDto> orders)
    {
        var sb = new StringBuilder();
        sb.AppendLine("ORDER DATA (most recent first):");
        foreach (var o in orders.OrderByDescending(x => x.CreatedAtUtc))
        {
            var total = o.Price * o.Quantity;
            sb.AppendLine($"- OrderId={o.Id}; ProductId={o.ProductId}; Product='{o.ProductName}'; Qty={o.Quantity}; UnitPrice={o.Price:0.00}; Total={total:0.00}; CreatedAtUtc={o.CreatedAtUtc:O}");
        }
       
        return sb.ToString();
    }
}