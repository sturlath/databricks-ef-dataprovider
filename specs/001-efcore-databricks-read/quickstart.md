# Quickstart (Phase 1)

## Goal
Execute a simple LINQ query against Databricks SQL using EFCore.Databricks provider (read-only).

## Prerequisites
- Simba Databricks ODBC driver installed
- Environment variables set:
  - DATABRICKS_CONNECTION_STRING or (DATABRICKS_DSN + DATABRICKS_TOKEN)
- .NET 9 SDK installed

## Steps
1. Add package (after first publish): `dotnet add package EFCore.Databricks --version 0.1.0`
2. Define a DbContext:
```csharp
public class AnalyticsContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseDatabricks();
}
public record Customer(int Id, string Name, DateTime CreatedUtc);
```
3. Run query:
```csharp
using var ctx = new AnalyticsContext();
var recent = ctx.Customers
    .Where(c => c.CreatedUtc > DateTime.UtcNow.AddDays(-7))
    .OrderBy(c => c.CreatedUtc)
    .Take(50)
    .Select(c => new { c.Id, c.Name })
    .ToList();
```
4. Observe log entry with SQL text and `[REDACTED]` parameters.

## Expected Outcome
- Query succeeds
- No tracking entities returned
- Unsupported query shapes (LEFT JOIN) throw NotSupportedException

-- End Quickstart --
