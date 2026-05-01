namespace CyFinance.Models.FundData;

/// <summary>
/// Aggregated snapshot of a mutual fund or ETF.
/// </summary>
public class FundSummary
{
    public string? Ticker { get; set; }
    public string? Name { get; set; }
    public string? FundFamily { get; set; }
    public string? Category { get; set; }
    public string? LegalType { get; set; }
    public double? Price { get; set; }
    public double? ExpenseRatio { get; set; }
    public double? StockPosition { get; set; }
    public double? BondPosition { get; set; }
    public List<FundTopHolding>? TopHoldings { get; set; }
    public List<FundSectorWeighting>? SectorWeightings { get; set; }
    public FundTrailingSummary? TrailingReturns { get; set; }
}

/// <summary>
/// Profile / metadata of a mutual fund or ETF.
/// </summary>
public class FundProfile
{
    public string? Ticker { get; set; }
    public string? FundFamily { get; set; }
    public string? Category { get; set; }
    public string? LegalType { get; set; }
    public string? ManagerName { get; set; }
    public double? ExpenseRatio { get; set; }
    public double? GrossExpenseRatio { get; set; }
    public double? FrontEndSalesLoad { get; set; }
}

/// <summary>
/// A single holding inside a fund.
/// </summary>
public class FundTopHolding
{
    public string? Symbol { get; set; }
    public string? Name { get; set; }
    public double? Percent { get; set; }
}

/// <summary>
/// Sector allocation weight for a fund.
/// </summary>
public class FundSectorWeighting
{
    public string? Sector { get; set; }
    public double? Weight { get; set; }
}

/// <summary>
/// Trailing period returns for a fund.
/// </summary>
public class FundTrailingSummary
{
    public double? OneMonth { get; set; }
    public double? ThreeMonth { get; set; }
    public double? Ytd { get; set; }
    public double? OneYear { get; set; }
    public double? ThreeYear { get; set; }
    public double? FiveYear { get; set; }
    public double? TenYear { get; set; }
}

/// <summary>
/// Annual return entry for a fund.
/// </summary>
public class FundAnnualReturnEntry
{
    public string? Year { get; set; }
    public double? ReturnPercent { get; set; }
}
