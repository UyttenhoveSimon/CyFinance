using System.Text.Json.Serialization;

namespace CyFinance.Models.CompanyNews;

/// <summary>
/// Top-level response model for Yahoo search-based company news.
/// </summary>
public class CompanyNewsResponse
{
    [JsonPropertyName("news")]
    public List<CompanyNewsItem>? News { get; set; }
}

/// <summary>
/// Represents a company news article associated with a ticker.
/// </summary>
public class CompanyNewsItem
{
    [JsonPropertyName("uuid")]
    public string? Uuid { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("publisher")]
    public string? Publisher { get; set; }

    [JsonPropertyName("link")]
    public string? Link { get; set; }

    [JsonPropertyName("providerPublishTime")]
    public long? ProviderPublishTime { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("relatedTickers")]
    public List<string>? RelatedTickers { get; set; }
}
