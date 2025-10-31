using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleWebApi.Controllers;
using SimpleWebApi.Core.Interfaces;
using SimpleWebApi.Core.Models;
using Xunit;

namespace SimpleWebApi.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _controller = new ProductsController(_mockProductService.Object);
    }

    [Fact]
    public async Task GetProducts_ReturnsOkResultWithProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Test Product 1", Price = 10.99m },
            new() { Id = 2, Name = "Test Product 2", Price = 20.99m }
        };
        _mockProductService.Setup(service => service.GetAllProductsAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsType<List<Product>>(okResult.Value);
        Assert.Equal(2, returnedProducts.Count);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product", Price = 15.99m };
        _mockProductService.Setup(service => service.GetProductByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<Product>(okResult.Value);
        Assert.Equal(1, returnedProduct.Id);
        Assert.Equal("Test Product", returnedProduct.Name);
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockProductService.Setup(service => service.GetProductByIdAsync(1)).ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreatedResult()
    {
        // Arrange
        var request = new CreateProductRequest 
        { 
            Name = "New Product", 
            Description = "New Description", 
            Price = 25.99m, 
            Stock = 10 
        };
        var createdProduct = new Product 
        { 
            Id = 1, 
            Name = request.Name, 
            Description = request.Description, 
            Price = request.Price, 
            Stock = request.Stock 
        };
        
        _mockProductService.Setup(service => service.CreateProductAsync(request)).ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.CreateProduct(request);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedProduct = Assert.IsType<Product>(createdResult.Value);
        Assert.Equal(1, returnedProduct.Id);
        Assert.Equal("New Product", returnedProduct.Name);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateProductRequest { Name = "", Price = -10m };
        _mockProductService.Setup(service => service.CreateProductAsync(request))
            .ThrowsAsync(new ArgumentException("Product name is required"));

        // Act
        var result = await _controller.CreateProduct(request);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ReturnsUpdatedProduct()
    {
        // Arrange
        var request = new UpdateProductRequest 
        { 
            Name = "Updated Product", 
            Description = "Updated Description", 
            Price = 30.99m, 
            Stock = 20 
        };
        var updatedProduct = new Product 
        { 
            Id = 1, 
            Name = request.Name, 
            Description = request.Description, 
            Price = request.Price, 
            Stock = request.Stock 
        };
        
        _mockProductService.Setup(service => service.UpdateProductAsync(1, request)).ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.UpdateProduct(1, request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<Product>(okResult.Value);
        Assert.Equal("Updated Product", returnedProduct.Name);
        Assert.Equal(30.99m, returnedProduct.Price);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_ReturnsNoContent()
    {
        // Arrange
        _mockProductService.Setup(service => service.DeleteProductAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteProduct(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        _mockProductService.Setup(service => service.DeleteProductAsync(1)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteProduct(1);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}