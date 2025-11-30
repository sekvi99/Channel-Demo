using ElasticDemo.Domain.Models;

namespace ElasticDemo.Domain.Interfaces;

public interface IElasticsearchService<T> where T : class
{
    // Index Management
    Task<bool> CreateIndexAsync(string indexName, CancellationToken cancellationToken = default);
    Task<bool> IndexExistsAsync(string indexName, CancellationToken cancellationToken = default);
    Task<bool> DeleteIndexAsync(string indexName, CancellationToken cancellationToken = default);

    // Document Operations
    Task<string> IndexDocumentAsync(T document, string? indexName = null, CancellationToken cancellationToken = default);
    Task<bool> UpdateDocumentAsync(string id, T document, string? indexName = null, CancellationToken cancellationToken = default);
    Task<T?> GetDocumentAsync(string id, string? indexName = null, CancellationToken cancellationToken = default);
    Task<bool> DeleteDocumentAsync(string id, string? indexName = null, CancellationToken cancellationToken = default);
    Task<int> BulkIndexDocumentsAsync(IEnumerable<T> documents, string? indexName = null, CancellationToken cancellationToken = default);

    // Search Operations
    Task<SearchResult<T>> SearchAsync(string query, string? indexName = null, CancellationToken cancellationToken = default);
    Task<SearchResult<T>> MultiMatchSearchAsync(string query, string[] fields, string? indexName = null, CancellationToken cancellationToken = default);
    Task<SearchResult<T>> AdvancedSearchAsync(ProductSearchRequest request, string? indexName = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> AutocompleteAsync(string prefix, string fieldName, int size = 5, string? indexName = null, CancellationToken cancellationToken = default);

    // Analytics
    Task<Dictionary<string, object>> GetAggregationsAsync(string? indexName = null, CancellationToken cancellationToken = default);

    // Health Check
    Task<bool> PingAsync(CancellationToken cancellationToken = default);
}