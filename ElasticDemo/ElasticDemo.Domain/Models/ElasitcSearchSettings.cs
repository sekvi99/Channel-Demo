namespace ElasticDemo.Domain.Models;

public sealed class ElasticsearchSettings
{
    public string Url { get; set; } = "http://localhost:9200";
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string DefaultIndex { get; set; } = "products";
    public string? CertificateFingerprint { get; set; }
    public string? CloudId { get; set; }
    public string? ApiKey { get; set; }
    public int TimeoutSeconds { get; set; } = 60;
    public bool EnableDebugMode { get; set; } = false;
}