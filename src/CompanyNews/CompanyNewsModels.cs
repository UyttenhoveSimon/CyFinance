using System.Text.Json.Serialization;

namespace CyFinance.Models.CompanyNews;

/// <summary>
/// The news section of Yahoo's search response, which is where company news comes from.
/// </summary>
public class CompanyNewsResponse
{
    [JsonPropertyName("news")]
    public List<CompanyNewsItem>? News { get; set; }
}

/// <summary>
/// A news article.
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

    /// <summary>
    /// When the publisher released the article, as a Unix timestamp in seconds.
    /// </summary>
    [JsonPropertyName("providerPublishTime")]
    public long? ProviderPublishTime { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("relatedTickers")]
    public List<string>? RelatedTickers { get; set; }
}
