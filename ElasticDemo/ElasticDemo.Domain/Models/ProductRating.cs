namespace ElasticDemo.Domain.Models;

public sealed record ProductRating
{
    public double Average { get; set; }
    public int Count { get; set; }
}