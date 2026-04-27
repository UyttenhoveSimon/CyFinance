namespace CyFinance.Models.StockScreening;

/// <summary>
/// Common yfinance predefined screener IDs.
/// </summary>
public static class PredefinedScreeners
{
    public const string AggressiveSmallCaps = "aggressive_small_caps";
    public const string DayGainers = "day_gainers";
    public const string DayLosers = "day_losers";
    public const string MostActives = "most_actives";
    public const string MostShortedStocks = "most_shorted_stocks";
    public const string GrowthTechnologyStocks = "growth_technology_stocks";
    public const string SmallCapGainers = "small_cap_gainers";
    public const string UndervaluedGrowthStocks = "undervalued_growth_stocks";
    public const string UndervaluedLargeCaps = "undervalued_large_caps";
    public const string ConservativeForeignFunds = "conservative_foreign_funds";
    public const string HighYieldBond = "high_yield_bond";
    public const string PortfolioAnchors = "portfolio_anchors";
    public const string SolidLargeBlendFunds = "solid_large_blend_funds";
    public const string SolidLargeGrowthFunds = "solid_large_growth_funds";
    public const string SolidMidcapGrowthFunds = "solid_midcap_growth_funds";
    public const string TopMutualFunds = "top_mutual_funds";
    public const string TopEtfsUs = "top_etfs_us";
    public const string TopPerformingEtfs = "top_performing_etfs";
    public const string TechnologyEtfs = "technology_etfs";
    public const string BondEtfs = "bond_etfs";

    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        AggressiveSmallCaps,
        DayGainers,
        DayLosers,
        MostActives,
        MostShortedStocks,
        GrowthTechnologyStocks,
        SmallCapGainers,
        UndervaluedGrowthStocks,
        UndervaluedLargeCaps,
        ConservativeForeignFunds,
        HighYieldBond,
        PortfolioAnchors,
        SolidLargeBlendFunds,
        SolidLargeGrowthFunds,
        SolidMidcapGrowthFunds,
        TopMutualFunds,
        TopEtfsUs,
        TopPerformingEtfs,
        TechnologyEtfs,
        BondEtfs,
    };

    public static bool IsKnown(string screenId)
    {
        if (string.IsNullOrWhiteSpace(screenId))
            return false;

        return All.Contains(screenId);
    }
}

/// <summary>
/// Base class for yfinance-style typed screener queries.
/// </summary>
public abstract class QueryBase
{
    private static readonly HashSet<string> AllowedOperators =
    [
        "EQ", "IS-IN", "BTWN", "GT", "LT", "GTE", "LTE", "AND", "OR"
    ];

    protected QueryBase(string @operator, params object[] operands)
    {
        Operator = (@operator ?? throw new ArgumentNullException(nameof(@operator))).ToUpperInvariant();
        Operands = operands?.ToList() ?? throw new ArgumentNullException(nameof(operands));

        Validate(Operator, Operands);
    }

    public string Operator { get; }

    public IReadOnlyList<object> Operands { get; }

    public ScreenerQueryNode ToNode() =>
        new()
        {
            Operator = Operator,
            Operands = Operands.Select(operand =>
            {
                return operand is QueryBase queryOperand
                    ? (object)queryOperand.ToNode()
                    : operand;
            }).ToList()
        };

    private static void Validate(string @operator, IReadOnlyList<object> operands)
    {
        if (!AllowedOperators.Contains(@operator))
            throw new ArgumentException($"Invalid operator '{@operator}'", nameof(@operator));

        if (operands.Count == 0)
            throw new ArgumentException("Operands cannot be empty", nameof(operands));

        switch (@operator)
        {
            case "AND":
            case "OR":
                if (operands.Count < 2 || operands.Any(o => o is not QueryBase))
                    throw new ArgumentException("AND/OR require at least two nested query operands", nameof(operands));
                break;
            case "EQ":
            case "GT":
            case "LT":
            case "GTE":
            case "LTE":
                if (operands.Count != 2)
                    throw new ArgumentException($"{@operator} requires exactly 2 operands", nameof(operands));
                break;
            case "BTWN":
                if (operands.Count != 3)
                    throw new ArgumentException("BTWN requires exactly 3 operands", nameof(operands));
                break;
            case "IS-IN":
                if (operands.Count < 2)
                    throw new ArgumentException("IS-IN requires at least 2 operands", nameof(operands));
                break;
        }
    }
}

public sealed class EquityQuery(string @operator, params object[] operands) : QueryBase(@operator, operands)
{
    public static EquityQuery Eq(string field, object value) => new("EQ", field, value);
    public static EquityQuery IsIn(string field, params object[] values) => new("IS-IN", PrependField(field, values));
    public static EquityQuery Btwn(string field, object min, object max) => new("BTWN", field, min, max);
    public static EquityQuery Gt(string field, object value) => new("GT", field, value);
    public static EquityQuery Lt(string field, object value) => new("LT", field, value);
    public static EquityQuery Gte(string field, object value) => new("GTE", field, value);
    public static EquityQuery Lte(string field, object value) => new("LTE", field, value);
    public static EquityQuery And(params EquityQuery[] queries) => new("AND", queries.Cast<object>().ToArray());
    public static EquityQuery Or(params EquityQuery[] queries) => new("OR", queries.Cast<object>().ToArray());

    private static object[] PrependField(string field, object[] values) => [field, ..values];
}

public sealed class FundQuery(string @operator, params object[] operands) : QueryBase(@operator, operands)
{
    public static FundQuery Eq(string field, object value) => new("EQ", field, value);
    public static FundQuery IsIn(string field, params object[] values) => new("IS-IN", PrependField(field, values));
    public static FundQuery Btwn(string field, object min, object max) => new("BTWN", field, min, max);
    public static FundQuery Gt(string field, object value) => new("GT", field, value);
    public static FundQuery Lt(string field, object value) => new("LT", field, value);
    public static FundQuery Gte(string field, object value) => new("GTE", field, value);
    public static FundQuery Lte(string field, object value) => new("LTE", field, value);
    public static FundQuery And(params FundQuery[] queries) => new("AND", queries.Cast<object>().ToArray());
    public static FundQuery Or(params FundQuery[] queries) => new("OR", queries.Cast<object>().ToArray());

    private static object[] PrependField(string field, object[] values) => [field, ..values];
}

public sealed class ETFQuery(string @operator, params object[] operands) : QueryBase(@operator, operands)
{
    public static ETFQuery Eq(string field, object value) => new("EQ", field, value);
    public static ETFQuery IsIn(string field, params object[] values) => new("IS-IN", PrependField(field, values));
    public static ETFQuery Btwn(string field, object min, object max) => new("BTWN", field, min, max);
    public static ETFQuery Gt(string field, object value) => new("GT", field, value);
    public static ETFQuery Lt(string field, object value) => new("LT", field, value);
    public static ETFQuery Gte(string field, object value) => new("GTE", field, value);
    public static ETFQuery Lte(string field, object value) => new("LTE", field, value);
    public static ETFQuery And(params ETFQuery[] queries) => new("AND", queries.Cast<object>().ToArray());
    public static ETFQuery Or(params ETFQuery[] queries) => new("OR", queries.Cast<object>().ToArray());

    private static object[] PrependField(string field, object[] values) => [field, ..values];
}