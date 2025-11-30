namespace ElasticDemo.Domain.Models;

public sealed record SearchHit<T>
{
    public T Document { get; set; } = default!;
    public double? Score { get; set; }
    public Dictionary<string, List<string>>? Highlights { get; set; }
}