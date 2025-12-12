using CustomerOrders.Api.Models;
using CustomerOrders.Api.Config;
using System.Net.Http.Json;
using Microsoft.Extensions.Options;

namespace CustomerOrders.Api.Services;

public interface IAiChatService
{
    Task<string> GetChatCompletionAsync(IReadOnlyList<ChatMessageDto> messages, CancellationToken cancellationToken = default);
}

public sealed class AiChatService : IAiChatService
{
    private readonly HttpClient _httpClient;
    private readonly OpenAiOptions _options;

    public AiChatService(HttpClient httpClient, OpenAiOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<string> GetChatCompletionAsync(IReadOnlyList<ChatMessageDto> messages, CancellationToken cancellationToken = default)
    {
       if(string.IsNullOrWhiteSpace(_options.ApiKey))
       {
           throw new InvalidOperationException("API key is not configured.");
       }

       _httpClient.BaseAddress ??= new Uri(_options.BaseUrl);
       _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);

       var payload = new
       {
           model = _options.Model,
           messages = messages.Select(m => new { role = m.Role, content = m.Content }).ToList()
       };

       Console.WriteLine($"Calling OpenAI at: {_httpClient.BaseAddress}chat/completions");


       using var response = await _httpClient.PostAsJsonAsync("chat/completions", payload, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Request to AI chat API failed with status code {response.StatusCode}: {errorContent}");
        }

        var json = await response.Content.ReadFromJsonAsync<ChatCompletionResponseJson>(cancellationToken);

        var reply = json?.Choices?.FirstOrDefault()?.Message?.Content;
        if (string.IsNullOrWhiteSpace(reply))
        {
            throw new InvalidOperationException("AI chat API returned an empty response.");
        }

        return reply;
    }

    public sealed class ChatCompletionResponseJson
    {
        public List<Choice>? Choices { get; set; }

        public sealed class Choice
        {
            public Message? Message { get; set; }
        }

        public sealed class Message
        {
            public string? Role { get; set; }
            public string? Content { get; set; }
        }
    }   
}