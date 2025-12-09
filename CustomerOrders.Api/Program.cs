using CustomerOrders.Api.Services;
using CustomerOrders.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IInMemoryStore, InMemoryStore>();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

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

app.MapPost("/api/orders", (int productId, int quantity, IInMemoryStore store) =>
{
    try
    {
        var order = store.AddOrder(productId, quantity);
        return Results.Created($"/api/orders/{order.Id}", order);
    }
    catch (InvalidOperationException ex)
    {
        return Results.BadRequest(ex.Message);
    }
})
.WithName("CreateOrder")
.WithOpenApi();

app.Run();