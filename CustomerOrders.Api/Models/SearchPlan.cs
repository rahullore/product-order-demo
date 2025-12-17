
namespace CustomerOrders.Api.Models;
public record SearchPlan(
    string Target, //product order
    string Ranking, // semantic min | max | sum
    string? RankingField //unit price totalQuantity
);