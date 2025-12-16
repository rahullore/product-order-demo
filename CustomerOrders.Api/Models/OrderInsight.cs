namespace CustomerOrders.Api.Models;

public record TopProductDto(string Name,int TotalQty,decimal TotalSpend);

public record OrderInsightDto(
    int TotalOrders,
    int TotalItems,
    decimal TotalSpend,
    List<TopProductDto> TopProducts,
    DateTimeOffset LastUpdateUTC);
