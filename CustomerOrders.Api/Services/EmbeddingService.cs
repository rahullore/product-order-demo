using System.Net.Http.Json;
using System.Text.Json;
using CustomerOrders.Api.Models;
using CustomerOrders.Api.Config;

namespace CustomerOrders.Api.Services;

public interface IEmbeddingService
{
    Task<float[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken = default);
}

public sealed class EmbeddingService : IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly EmbeddingOptions _options;

    public EmbeddingService(HttpClient httpClient, EmbeddingOptions options)
    {
        _httpClient = httpClient;
        _options = options;
    }

    public async Task<float[]> GetEmbeddingAsync(string text, CancellationToken cancellationToken = default)
    {
        if(string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be null or empty.", nameof(text));
        }

        var request = new
        {
            input = text,
            model = _options.Model
        };

        using var httpReq = new HttpRequestMessage(HttpMethod.Post,$"{_options.BaseUrl}embeddings");

        httpReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);

        httpReq.Content = JsonContent.Create(request);

        using var resp = await _httpClient.SendAsync(httpReq, cancellationToken);

        if(!resp.IsSuccessStatusCode)
        {
            var errorContent = await resp.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpRequestException($"Error fetching embedding: {resp.StatusCode}, Details: {errorContent}");
        }

        using var json = JsonDocument.Parse(await resp.Content.ReadAsStringAsync(cancellationToken));
        var embedding = json.RootElement
            .GetProperty("data")[0]
            .GetProperty("embedding")
            .EnumerateArray()
            .Select(e => e.GetSingle())
            .ToArray();

        return embedding;
    }
    
}