using System.Text.Json;
using CustomerOrders.Api.Models;

namespace CustomerOrders.Api.Services;

public interface ISearchPlanner
{
    Task<SearchPlan> CreateSearchPlanAsync(string query, CancellationToken cancellationToken = default);
    decimal GetDecimal(Dictionary<string, string> metadata, string? key);
    
}

public sealed class LlmSearchPlanner : ISearchPlanner
{
    private readonly IAiChatService _aiChatService;

    public LlmSearchPlanner(IAiChatService aiChatService)
    {
        _aiChatService = aiChatService;
    }

    public async Task<SearchPlan> CreateSearchPlanAsync(string query, CancellationToken cancellationToken = default)
    {
        var messages = new List<ChatMessageDto>{
            new ChatMessageDto("system",
                @"You are a search planner for a customer orders system.

                Return STRICT JSON only:
                {
                ""target"": ""product"" | ""order"",
                ""ranking"": ""semantic"" | ""min"" | ""max"" | ""sum"",
                ""rankingField"": string | null
                }

                Rules:
                - min = cheapest / lowest
                - max = most / highest
                - semantic = no numeric comparison
                - rankingField must match stored metadata keys
                "),
            new ChatMessageDto("user", query)
        };

        var raw = await _aiChatService.GetChatCompletionAsync(messages, cancellationToken);
        Console.WriteLine($"LLM Search Plan Response: {raw}");
        var plan = JsonSerializer.Deserialize<SearchPlan>(raw, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return plan ?? new SearchPlan("product", "semantic", null);
    }

    public decimal GetDecimal(Dictionary<string, string> metadata, string? key)
    {
        if (key == null) return decimal.MaxValue;
        return metadata.TryGetValue(key, out var s) && decimal.TryParse(s, out var d)
            ? d
            : decimal.MaxValue;
    }
}