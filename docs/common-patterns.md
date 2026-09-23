# Common Patterns

The library is organized around feature-specific services and a facade client.

Use the facade when you want a single entry point:

```csharp
var news = await client.CompanyNews.GetLatestCompanyNewsAsync("AAPL");
```

Use the service interface directly when you want to wire up a narrower dependency surface in your own code.

Most APIs return strongly typed models and follow the same async pattern, which keeps the surface area predictable across features.

## Counts are a ceiling, not a promise

Yahoo's endpoints cap a result at the count you asked for and only then drop the
items they consider redundant, so a call routinely answers with fewer rows than it
was asked for. Company news is where this bites hardest, because a widely covered
ticker produces many near-identical stories:

| Request | AAPL | MSFT | TSLA |
| :--- | :---: | :---: | :---: |
| `newsCount: 1` | 0–1 | 1 | 1 |
| `newsCount: 5` | 3 | 5 | 5 |
| `newsCount: 10` | 8 | 8 | 9 |

A request for a single item is the one to avoid: it leaves no slack for
deduplication to take anything away from, so the call comes back empty whenever the
one story Yahoo picked is one it also decided to drop. Ask for a window and take what
you need from it.

```csharp
// Fragile: an empty result here means "Yahoo deduplicated it away", not "no news".
var latest = (await client.CompanyNews.GetCompanyNewsAsync("AAPL", 1))?.FirstOrDefault();

// Reliable, and what GetLatestCompanyNewsAsync does for you.
var news = await client.CompanyNews.GetCompanyNewsAsync("AAPL", 5);
var newest = news?.FirstOrDefault();
```

The same caution applies to anything that filters a window client side, such as
`GetCompanyNewsSinceAsync`: an empty list means nothing recent was in what Yahoo
sent, not that nothing was published.
