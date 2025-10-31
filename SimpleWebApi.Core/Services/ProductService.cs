using SimpleWebApi.Core.Interfaces;
using SimpleWebApi.Core.Models;

namespace SimpleWebApi.Core.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        if (id <= 0) throw new ArgumentException("Invalid product ID");
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<Product> CreateProductAsync(CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Product name is required");

        if (request.Price <= 0)
            throw new ArgumentException("Price must be greater than 0");

        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Stock = request.Stock,
            CreatedAt = DateTime.UtcNow
        };

        return await _productRepository.CreateAsync(product);
    }

    public async Task<Product?> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null) return null;

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Product name is required");

        if (request.Price <= 0)
            throw new ArgumentException("Price must be greater than 0");

        existingProduct.Name = request.Name;
        existingProduct.Description = request.Description;
        existingProduct.Price = request.Price;
        existingProduct.Stock = request.Stock;
        existingProduct.UpdatedAt = DateTime.UtcNow;

        return await _productRepository.UpdateAsync(existingProduct);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        return await _productRepository.DeleteAsync(id);
    }

    public async Task<List<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await _productRepository.GetAllAsync();

        return await _productRepository.SearchAsync(searchTerm);
    }
}