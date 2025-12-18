namespace CustomerOrders.Api.Models;

public sealed record ProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int Stock
);

public sealed record CreateProductRequest(
    string Name,
    string Description,
    decimal Price,
    int Stock
);
