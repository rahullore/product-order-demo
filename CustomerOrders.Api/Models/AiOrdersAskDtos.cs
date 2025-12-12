namespace CustomerOrders.Api.Models;

public sealed record OrderAskRequestDto(
    string Question,
    int? TakeLastN=10,
    IReadOnlyList<int>? OrderIds = null
);

public sealed record OrderAskResponseDto(
    string Answer,
    int OrdersIncluded,
    IReadOnlyList<int> UsedOrderIds
    );