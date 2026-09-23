# Common Patterns

The library is organized around feature-specific services and a facade client.

Use the facade when you want a single entry point:

```csharp
var news = await client.CompanyNews.GetLatestCompanyNewsAsync("AAPL");
```

Use the service interface directly when you want to wire up a narrower dependency surface in your own code.

Most APIs return strongly typed models and follow the same async pattern, which keeps the surface area predictable across features.

## Item counts

Counts like `newsCount` are an upper bound. Yahoo trims the result to the number
you asked for and then drops the stories it considers duplicates, so you normally
get fewer items back than you asked for. How many fewer depends on the ticker --
AAPL attracts far more near-identical coverage than MSFT or TSLA:

| Requested | AAPL | MSFT | TSLA |
| :--- | :---: | :---: | :---: |
| 1 | 0-1 | 1 | 1 |
| 5 | 3 | 5 | 5 |
| 10 | 8 | 8 | 9 |

Asking for a single item is the case to avoid. Nothing is left to fall back on, so
if Yahoo drops the one story it picked you get an empty list instead of the latest
headline. Ask for a handful and take the first, which is what
`GetLatestCompanyNewsAsync` does for you.

```csharp
// Empty whenever Yahoo deduplicates away the one story it picked.
var latest = (await client.CompanyNews.GetCompanyNewsAsync("AAPL", 1))?.FirstOrDefault();

// Same thing, but it actually comes back with something.
var news = await client.CompanyNews.GetCompanyNewsAsync("AAPL", 5);
var newest = news?.FirstOrDefault();
```

`GetCompanyNewsSinceAsync` filters that same window in memory, so an empty list
there means nothing recent came back, not that nothing was published.
