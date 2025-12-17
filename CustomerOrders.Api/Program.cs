using CustomerOrders.Api.Services;
using CustomerOrders.Api.Models;
using CustomerOrders.Api.Config;
using DotNetEnv;
using System.Data.Common;
using System.IO.Pipelines;
using System.Security.Cryptography.X509Certificates;
using Microsoft.VisualBasic;
using System.Net.Security;
using System.Xml.Schema;
using System.IO.Pipes;
using System.Runtime.CompilerServices;
using System.Runtime;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.Marshalling;
using System.Net.WebSockets;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Linq.Expressions;
using System.Xml;
using System.ComponentModel;
using System.Diagnostics.SymbolStore;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Data;
using System.Reflection.Emit;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNuxt", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
}); 

builder.Services.AddSingleton<IInMemoryStore, InMemoryStore>();
builder.Services.AddHttpClient<IAiChatService, AiChatService>();
builder.Services.AddSingleton<IOrderContextBuilder, OrderContextBuilder>();
builder.Services.AddSingleton<RagService>();
builder.Services.AddSingleton<RagStore>();
builder.Services.AddHttpClient<IEmbeddingService, EmbeddingService>();
builder.Services.AddSingleton<IInMemoryVectorStore, InMemoryVectorStore>();     
//builder.Services.Configure<OpenAiOptions>(builder.Configuration.GetSection("OpenAi"));
//load .env variable
var openAiOptions = new OpenAiOptions
{
    ApiKey = Environment.GetEnvironmentVariable("API_KEY"),
    BaseUrl = Environment.GetEnvironmentVariable("BASE_URL"),
    Model = Environment.GetEnvironmentVariable("MODEL")
};
builder.Services.AddSingleton(openAiOptions);

var embeddingOptions = new EmbeddingOptions
{
    ApiKey = Environment.GetEnvironmentVariable("EMBEDDED_API_KEY"),
    BaseUrl = Environment.GetEnvironmentVariable("EMBEDDED_BASE_URL"),
    Model = Environment.GetEnvironmentVariable("EMBEDDED_MODEL")
};
builder.Services.AddSingleton(embeddingOptions);

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseCors("AllowNuxt");

app.MapGet("/api/products", (IInMemoryStore store) =>
{
    return Results.Ok(store.GetProducts());
})
.WithName("GetProducts")
.WithOpenApi();

app.MapGet("/api/products/{id}", (int id, IInMemoryStore store) =>
{
    var product = store.GetProduct(id);
    return product is not null ? Results.Ok(product) : Results.NotFound();
})
.WithName("GetProductById")
.WithOpenApi();

app.MapGet("/api/orders", (IInMemoryStore store) =>
{
    return Results.Ok(store.GetOrders());
})
.WithName("GetOrders")
.WithOpenApi();

app.MapGet("/api/orders/{id}", (int id, IInMemoryStore store) =>
{
    var order = store.GetOrder(id);
    return order is not null ? Results.Ok(order) : Results.NotFound();
})
.WithName("GetOrderById")
.WithOpenApi();

app.MapPost("/api/orders", (CreateOrderRequest request, IInMemoryStore store) =>
{
    try
    {
        var order = store.AddOrder(request.ProductId, request.Quantity);
        return Results.Created($"/api/orders/{order.Id}", order);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("CreateOrder")
.WithOpenApi();

app.MapDelete("/api/orders", (IInMemoryStore store) =>
{
    store.ClearOrders();
    return Results.NoContent();
}).WithName("ClearOrders")
.WithOpenApi();

app.MapPost("/api/ai/chat", async(ChatRequestDto request,IAiChatService aiChatService, CancellationToken cancellationToken) =>
{
    if(request == null || request.Messages == null || !request.Messages.Any())
    {
        return Results.BadRequest("Invalid chat request.");
    }
    try
    {
        var reply = await aiChatService.GetChatCompletionAsync(request.Messages, cancellationToken);
        var response = new ChatResponseDto(reply);
        return Results.Ok(response);    
    }
    catch(InvalidOperationException ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError
        );
    }
})
.WithName("AiChat")
.WithOpenApi();



app.MapPost("/api/ai/orders/ask", async (
    OrderAskRequestDto req,
    IInMemoryStore store,
    IOrderContextBuilder contextBuilder,
    IAiChatService ai,
    CancellationToken ct)=>{

        if(string.IsNullOrWhiteSpace(req?.Question))
        {
            return Results.BadRequest("Question is required.");
        }

        IReadOnlyList<OrderDto> selectedOrders;

        if(req.OrderIds is {Count:>0})
        {
            selectedOrders = req.OrderIds
            .Select(id=> store.GetOrder(id))
            .Where(o=> o != null)
            .Cast<OrderDto>()
            .ToList();
        }
        else
        {
             var take = Math.Clamp(req.TakeLastN ?? 10, 1, 50);
             selectedOrders = store.GetOrders()
            .OrderByDescending(o => o.CreatedAtUtc)
            .Take(take)
            .ToList();
        }

        var context = contextBuilder.BuildOrderContext(selectedOrders);

        var message = new List<ChatMessageDto>
        {
            new ChatMessageDto("system","You are an assistant for a Customer Orders system. " +
                        "Answer using ONLY the provided ORDER DATA. " +
                        "If the answer isn't in the data, say you don't have enough information."
                ),
            new ChatMessageDto("user", context + "\n\nQuestion: " + req.Question)
        };

        var answer = await ai.GetChatCompletionAsync(message, ct);

        return Results.Ok(new OrderAskResponseDto(
            Answer: answer,
            OrdersIncluded: selectedOrders.Count,
            UsedOrderIds:  selectedOrders.Select(o=>o.Id).ToList()
            ));
    }
)
.WithName("AiOrderAsk")
.WithOpenApi();

app.MapPost("/api/rag/ingest/orders", (IInMemoryStore store, RagStore ragStore) =>
{
    var orders = store.GetOrders();
    var docs = orders.Select(o => new RagDocument(
        Id: $"order-{o.Id}",
        Text: $"Order {o.Id}: product={o.ProductName}, qty={o.Quantity}, unitPrice={o.Price}, createdAt={o.CreatedAtUtc}",
        Metadata: new Dictionary<string, string>
        {
            {"Type", "Order" },
            {"OrderId", o.Id.ToString() }
        }
    )).ToList();
    ragStore.UpsertMany(docs);
    return Results.Ok(new { IngestedCount = orders.Count, RagStore = ragStore.GetAll()  });
})
.WithName("RagIngestOrders")
.WithOpenApi();

app.MapPost("/api/rag/ask", async(
    RagAskRequest req,
    RagStore ragStore,
    RagService ragService,
    IAiChatService ai,
    CancellationToken ct) =>
    {
        var topK = Math.Clamp(req.TopK ?? 5, 1, 20);
        var allDocs = ragStore.GetAll();

        var top = ragService.Retrieve(req.Question, allDocs, topK);

        if(top.Count == 0)
        {
            return Results.Ok(new RagAskResponse(
                Answer: "No matching orders found in the ingested data. Try rephrasing your question or ingest orders again.",
                TopK: topK,
                Sources: Array.Empty<object>()
            ));
        }

        var context = ragService.BuildContext(top);
        
        
        var message = new List<ChatMessageDto>
        {
            new("system", "You are a helpful assistant for a customer orders system. Use ONLY the provided context. If the context is insufficient, say what is missing."),
            new("system", $"CONTEXT:\n{context}"),
            new("user", req.Question)
        };
        var answer = await ai.GetChatCompletionAsync(message, ct);

        var sources = top
            .Select(d => new
            {
                d.Id,
                d.Metadata,
                Preview = d.Text.Length > 160 ? d.Text[..160] + "..." : d.Text
            })
            .ToList();

        return Results.Ok(new RagAskResponse(
            Answer: answer,
            TopK: topK,
            Sources: sources
        ));
    })
    .WithName("RagAsk")
    .WithOpenApi();

app.MapDelete("/api/rag/clear", (RagStore ragStore) =>
{
    ragStore.Clear();
    return Results.NoContent();
})
.WithName("RagClear")
.WithOpenApi();

app.MapGet("/api/orders/insights", (IInMemoryStore store) =>
{
    var orders = store.GetOrders();

    var totalOrders = orders.Count;
    var totalItems = orders.Sum(o => o.Quantity);
    var totalSpend = orders.Sum(o => o.Quantity * o.Price);

    var topProducts = orders
        .GroupBy(o => o.ProductName)
        .Select(g => new TopProductDto(
            Name: g.Key,
            TotalQty: g.Sum(o => o.Quantity),
            TotalSpend: g.Sum(o => o.Quantity * o.Price)
        ))
        .OrderByDescending(tp => tp.TotalSpend)
        .Take(5)
        .ToList();

    var insight = new OrderInsightDto(
        TotalOrders: totalOrders,
        TotalItems: totalItems,
        TotalSpend: totalSpend,
        TopProducts: topProducts,
        LastUpdateUTC: DateTimeOffset.UtcNow
    );

    return Results.Ok(insight);
})
.WithName("GetOrderInsights")
.WithOpenApi();

app.MapPost("/api/vector/index/orders", async (IInMemoryStore store, 
    IInMemoryVectorStore vectorStore, 
    IEmbeddingService embeddingService, 
    CancellationToken ct) =>
{
    var orders =  store.GetOrders();

    foreach(var order in orders)
    {
        var text = $"product={order.ProductName}, totalQuantity={order.Quantity}, unitPrice={order.Price}";
        var embedding = await embeddingService.GetEmbeddingAsync(text, ct);
        var record = new VectorRecord(
            Id: $"order-{order.Id}",
            Vector: embedding,
            Text: text,
            Metadata: new Dictionary<string, string>
            {
                ["type"] = "order",
                ["productName"] = order.ProductName,
                ["unitPrice"] = order.Price.ToString(),
                ["quantity"] = order.Quantity.ToString()
            
            }
        );
        vectorStore.UpsertVector(record);
    }

    return Results.Ok(new {indexed = orders.Count, totalVectors = vectorStore.GetAllVectors().Count });
}).
WithName("VectorIndexOrders").
WithOpenApi();

app.MapPost("/api/vector/search", async(
    VectorSearchRequest req,
    IEmbeddingService embeddingService,
    IInMemoryVectorStore vectorStore,
    CancellationToken cancellationToken) =>
{
    var qVec = await embeddingService.GetEmbeddingAsync(req.Query, cancellationToken);
    var results = vectorStore.GetAllVectors()
        .Select(v => new
        {
           v.Id,
           v.Text,
           Score = VectorMath.CosineSimilarity(qVec, v.Vector)
        })
        .OrderByDescending(x => x.Score)
        .Take(req.TopK)
        .ToList();

    return Results.Ok(results);
  
})
.WithName("VectorSearch").
WithOpenApi();

app.UseForwardedHeaders();

app.Run();