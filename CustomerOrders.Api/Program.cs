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
//builder.Services.Configure<OpenAiOptions>(builder.Configuration.GetSection("OpenAi"));
//load .env variable
var openAiOptions = new OpenAiOptions
{
    ApiKey = Environment.GetEnvironmentVariable("API_KEY"),
    BaseUrl = Environment.GetEnvironmentVariable("BASE_URL"),
    Model = Environment.GetEnvironmentVariable("MODEL")
};
builder.Services.AddSingleton(openAiOptions);

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

app.UseForwardedHeaders();

app.Run();