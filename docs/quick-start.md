# Quick Start

Install the service collection extension and resolve the facade client from dependency injection.

```csharp
using CyFinance.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();
services.AddCyFinance();

await using var provider = services.BuildServiceProvider();
var client = provider.GetRequiredService<CyFinanceClient>();
```

From there, call the specific service you need:

```csharp
var quote = await client.QuoteSummary.GetQuoteSummaryAsync("AAPL", "price", "summaryDetail");
var prices = await client.HistoricalData.GetHistoricalPricesAsync("MSFT");
```