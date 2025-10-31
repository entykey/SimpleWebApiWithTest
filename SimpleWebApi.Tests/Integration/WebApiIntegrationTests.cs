using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace SimpleWebApi.Tests.Integration;

public class WebApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public WebApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ReturnsHealthy()
    {
        // Act
        var response = await _client.GetAsync("/api/health");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        Assert.NotNull(content);
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessWithProducts()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreated()
    {
        // Arrange
        var product = new
        {
            name = "Integration Test Product",
            description = "Product created during integration test",
            price = 29.99m,
            stock = 25
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", product);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var location = response.Headers.Location;
        Assert.NotNull(location);
        
        var createdResponse = await _client.GetAsync(location);
        createdResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task GetProduct_WithNonExistingId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/products/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}