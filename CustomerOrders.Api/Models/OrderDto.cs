namespace CustomerOrders.Api.Models;

public sealed record OrderDto(
    int Id,
    int ProductId,
    string ProductName,
    int Quantity,
    decimal Price,
    DateTime CreatedAtUtc
);

public sealed record CreateOrderRequest(
    int ProductId,
    int Quantity
);
