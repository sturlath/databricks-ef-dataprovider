# databricks-ef-dataprovider

An Entity Framework Core relational provider (EFCore.Databricks) for read-only LINQ queries against Databricks SQL via the Simba ODBC driver (System.Data.Odbc).

Status: v0.1.0 planning complete (spec/plan/tasks in `specs/001-efcore-databricks-read/`). Implementation will follow tests-first.

## What this is
- An EF Core provider you configure with `UseDatabricks(...)`, just like other providers listed in the EF Core providers catalog.
- Scope: SELECT with projection, WHERE, ORDER BY, GROUP BY + aggregates, INNER JOIN, LIMIT n. Read-only by design (no DDL/DML/SaveChanges).

## Quick usage (planned API)
```csharp
using Microsoft.EntityFrameworkCore;

public class AnalyticsContext : DbContext
{
	public DbSet<Customer> Customers => Set<Customer>();
	protected override void OnConfiguring(DbContextOptionsBuilder options)
	{
		// Option 1: Resolve from environment (DATABRICKS_CONNECTION_STRING or DATABRICKS_DSN + DATABRICKS_TOKEN)
		options.UseDatabricks();

		// Option 2: Explicit connection string
		// options.UseDatabricks("DSN=MyDatabricksWarehouse;");

		// Option 3: Full ODBC connection string (example fields vary by Simba driver)
		// options.UseDatabricks("Driver={Simba Spark ODBC Driver};Host=<host>;Port=443;HTTPPath=<http-path>;UID=token;PWD=<token>");
	}
}

public record Customer(int Id, string Name, DateTime CreatedUtc);
```

Note: `Data Source=:memory:` is specific to in-memory providers and not applicable to Databricks ODBC.

## Read more
- Spec: `specs/001-efcore-databricks-read/spec.md`
- Plan: `specs/001-efcore-databricks-read/plan.md`
- Tasks: `specs/001-efcore-databricks-read/tasks.md`

## Out of scope (v0.1.0)
- SaveChanges, DDL/migrations, OFFSET pagination, window functions, CROSS/RIGHT/FULL joins, UDFs.