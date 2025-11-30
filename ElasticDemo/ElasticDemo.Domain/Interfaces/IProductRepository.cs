using ElasticDemo.Domain.Models;

namespace ElasticDemo.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(string id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<string> AddAsync(Product product);
    Task<bool> UpdateAsync(string id, Product product);
    Task<bool> DeleteAsync(string id);
    Task<SearchResult<Product>> SearchAsync(ProductSearchRequest request);
}