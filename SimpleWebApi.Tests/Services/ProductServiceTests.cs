using Moq;
using SimpleWebApi.Core.Interfaces;
using SimpleWebApi.Core.Models;
using SimpleWebApi.Core.Services;
using Xunit;

namespace SimpleWebApi.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _productService = new ProductService(_mockRepository.Object);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidData_ReturnsProduct()
    {
        // Arrange
        var request = new CreateProductRequest 
        { 
            Name = "Test Product", 
            Description = "Test Description", 
            Price = 19.99m, 
            Stock = 15 
        };
        var product = new Product 
        { 
            Id = 1, 
            Name = request.Name, 
            Description = request.Description, 
            Price = request.Price, 
            Stock = request.Stock 
        };
        
        _mockRepository.Setup(repo => repo.CreateAsync(It.IsAny<Product>())).ReturnsAsync(product);

        // Act
        var result = await _productService.CreateProductAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal(19.99m, result.Price);
        _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_WithEmptyName_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateProductRequest { Name = "", Price = 10m };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _productService.CreateProductAsync(request));
        _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task CreateProductAsync_WithInvalidPrice_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateProductRequest { Name = "Test Product", Price = -5m };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _productService.CreateProductAsync(request));
        _mockRepository.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var product = new Product { Id = 1, Name = "Test Product", Price = 15.99m };
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _productService.GetProductByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Test Product", result.Name);
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetProductByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateProductAsync_WithValidData_ReturnsUpdatedProduct()
    {
        // Arrange
        var existingProduct = new Product { Id = 1, Name = "Old Name", Price = 10m };
        var request = new UpdateProductRequest { Name = "New Name", Price = 20m, Stock = 5 };
        
        _mockRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existingProduct);
        _mockRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(existingProduct);

        // Act
        var result = await _productService.UpdateProductAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
        Assert.Equal(20m, result.Price);
    }

    [Fact]
    public async Task SearchProductsAsync_WithSearchTerm_ReturnsFilteredProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m }
        };
        _mockRepository.Setup(repo => repo.SearchAsync("laptop")).ReturnsAsync(products);

        // Act
        var result = await _productService.SearchProductsAsync("laptop");

        // Assert
        Assert.Single(result);
        Assert.Equal("Laptop", result[0].Name);
    }
}