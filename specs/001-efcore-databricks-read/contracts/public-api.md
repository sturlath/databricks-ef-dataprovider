# Contract: Public API Surface (Planned v0.1.0)

```csharp
namespace Microsoft.EntityFrameworkCore;

public static class DatabricksDbContextOptionsExtensions
{
    public static DbContextOptionsBuilder UseDatabricks(
        this DbContextOptionsBuilder optionsBuilder,
        string? connectionString = null,
        Action<DatabricksOptions>? databricksOptionsAction = null);
}

public sealed class DatabricksOptions
{
    public string? Dsn { get; set; }
    public string? Token { get; set; }
    public bool EnableLeftJoinPreview { get; set; } // ignored in v0.1.0
}
```

Behavioral Contract:
- If connectionString null → attempt env: DATABRICKS_CONNECTION_STRING then DATABRICKS_DSN (+ DATABRICKS_TOKEN).
- Throws InvalidOperationException if resolution fails.
- Always configures AsNoTrackingQueryTrackingBehavior.
- Does not mutate existing logging configuration.
