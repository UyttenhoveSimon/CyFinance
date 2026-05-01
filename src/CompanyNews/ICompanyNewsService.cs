using CyFinance.Models.CompanyNews;

namespace CyFinance.Services.CompanyNews;

/// <summary>
/// Interface for retrieving ticker-specific company news.
/// </summary>
public interface ICompanyNewsService
{
    Task<List<CompanyNewsItem>?> GetCompanyNewsAsync(string ticker, int newsCount = 10);
    Task<CompanyNewsItem?> GetLatestCompanyNewsAsync(string ticker);
    Task<List<CompanyNewsItem>?> GetCompanyNewsSinceAsync(string ticker, long sinceUnixTime, int newsCount = 25);
}
