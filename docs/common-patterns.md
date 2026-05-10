# Common Patterns

The library is organized around feature-specific services and a facade client.

Use the facade when you want a single entry point:

```csharp
var news = await client.CompanyNews.GetLatestCompanyNewsAsync("AAPL");
```

Use the service interface directly when you want to wire up a narrower dependency surface in your own code.

Most APIs return strongly typed models and follow the same async pattern, which keeps the surface area predictable across features.