namespace ElasticDemo.Domain.Models;

public sealed record SearchResult<T>
{
    public long Total { get; set; }
    public long Took { get; set; }
    public IReadOnlyCollection<T> Documents { get; set; } = Array.Empty<T>();
    public IReadOnlyCollection<SearchHit<T>> Hits { get; set; } = Array.Empty<SearchHit<T>>();
}