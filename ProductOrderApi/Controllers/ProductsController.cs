using Microsoft.AspNetCore.Mvc;
using ProductOrderApi.Models;

namespace ProductOrderApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private static List<Product> _products = new()
    {
        new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, Stock = 50, ImageUrl = "https://via.placeholder.com/150?text=Laptop" },
        new Product { Id = 2, Name = "Smartphone", Description = "Latest smartphone model", Price = 699.99m, Stock = 100, ImageUrl = "https://via.placeholder.com/150?text=Phone" },
        new Product { Id = 3, Name = "Headphones", Description = "Noise-cancelling headphones", Price = 199.99m, Stock = 75, ImageUrl = "https://via.placeholder.com/150?text=Headphones" },
        new Product { Id = 4, Name = "Tablet", Description = "Portable tablet device", Price = 449.99m, Stock = 30, ImageUrl = "https://via.placeholder.com/150?text=Tablet" },
        new Product { Id = 5, Name = "Smart Watch", Description = "Fitness tracking smartwatch", Price = 299.99m, Stock = 60, ImageUrl = "https://via.placeholder.com/150?text=Watch" }
    };

    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetProducts()
    {
        return Ok(_products);
    }

    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(product);
    }

    [HttpPost]
    public ActionResult<Product> CreateProduct(Product product)
    {
        product.Id = _products.Max(p => p.Id) + 1;
        _products.Add(product);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, Product product)
    {
        var existingProduct = _products.FirstOrDefault(p => p.Id == id);
        if (existingProduct == null)
        {
            return NotFound();
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Stock = product.Stock;
        existingProduct.ImageUrl = product.ImageUrl;

        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        _products.Remove(product);
        return NoContent();
    }
}
