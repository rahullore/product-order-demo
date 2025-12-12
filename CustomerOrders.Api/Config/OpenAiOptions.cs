namespace CustomerOrders.Api.Config;

public sealed class OpenAiOptions
{
    public const string SectionName = "OpenAi";
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.perplexity.ai/";
    public string Model { get; set; } = "sonar-large-online";
}