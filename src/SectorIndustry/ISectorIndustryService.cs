using CyFinance.Models.SectorIndustry;

namespace CyFinance.Services.SectorIndustry;

/// <summary>
/// Provides sector and industry classification, and the members of each.
/// </summary>
public interface ISectorIndustryService
{
    /// <summary>
    /// Gets the sector and industry a ticker is classified under.
    /// </summary>
    Task<SectorIndustryInfo?> GetSectorInfoAsync(string ticker);

    /// <summary>
    /// Gets stocks in a sector, by running a screener query.
    /// </summary>
    /// <param name="sector">A Yahoo sector name, for example "Technology".</param>
    Task<List<SectorScreenerEntry>> GetStocksInSectorAsync(string sector, int size = 25);

    /// <summary>
    /// Gets stocks in an industry, by running a screener query.
    /// </summary>
    /// <param name="industry">
    /// A Yahoo industry name, for example "Software—Application". Several of these are
    /// spelled with an em dash, and the name has to match Yahoo's spelling exactly.
    /// </param>
    Task<List<SectorScreenerEntry>> GetStocksInIndustryAsync(string industry, int size = 25);
}
