# Copilot Instructions for Databricks EF Core Provider

## Repository Overview
This repository contains an Entity Framework Core relational provider (EFCore.Databricks) for read-only LINQ queries against Databricks SQL via the Simba ODBC driver (System.Data.Odbc). 

## Technology Stack
- **.NET 9**: This project targets .NET 9 with forward compatibility for .NET 9 for the latest performance and language features
- **Entity Framework Core 9**: Using the latest EF Core 9.0.8 for modern data access patterns and performance improvements
- **ODBC**: Uses System.Data.Odbc for Databricks connectivity via Simba driver
- **xUnit**: For unit testing with FakeItEasy for mocking

## Project Structure
- `src/EFCore.Databricks/`: Main provider implementation
- `tests/EFCore.Databricks.Tests/`: Unit tests using contract-based testing
- `specs/001-efcore-databricks-read/`: Detailed specifications and plans

## Key Components
- **DatabricksOptionsExtension**: Inherits from RelationalOptionsExtension for proper EF Core integration
- **DatabricksLoggingDefinitions**: Concrete implementation of RelationalLoggingDefinitions
- **Service Registration**: Proper DI setup for EF Core relational services

## Scope & Constraints
- **READ-ONLY**: No SaveChanges, DDL, or migrations support
- **Query Types**: SELECT with projection, WHERE, ORDER BY, GROUP BY, aggregates, INNER JOIN, LIMIT
- **Connection**: ODBC via Simba Spark ODBC Driver
- **Environment**: Databricks SQL Warehouses

## Development Guidelines
- Follow EF Core provider patterns and conventions
- Maintain test-first development approach
- Keep changes minimal and focused
- Use .NET 9 features where beneficial for performance or code clarity
- Ensure all dependency injection is properly configured for EF Core

## Testing Strategy
Tests are organized by contract/feature:
- QuickstartScenarioTests: Basic configuration scenarios
- SelectProjectionTests: Basic SELECT queries
- SelectWhereTests: WHERE clause translation
- SelectOrderByLimitTests: ORDER BY and LIMIT support
- SelectGroupByAggregateTests: GROUP BY with aggregates
- InnerJoinTests: INNER JOIN translation

Current test status: Tests pass dependency injection and reach EF Core query translation phase.
=======
# Databricks Entity Framework Core Data Provider

Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.

This is a .NET 9 C# library project that provides an Entity Framework Core data provider for read-only LINQ queries against Databricks SQL via the Simba ODBC driver. The project is currently in early development (v0.1.0) with a test-driven approach where tests exist before implementation.

## Working Effectively

### Bootstrap and Build
- **Prerequisites**: .NET 9 SDK is required. Verify with `dotnet --version` (should show 8.0.x).
- **Initial setup**:
  - `dotnet restore EFCore.Databricks.sln` -- takes ~20 seconds. NEVER CANCEL. Set timeout to 60+ seconds.
  - `dotnet build EFCore.Databricks.sln --no-restore -c Release` -- takes ~8 seconds. NEVER CANCEL. Set timeout to 30+ seconds.
- **Development build**:
  - `dotnet build EFCore.Databricks.sln --no-restore -c Debug` -- faster debug build, ~2 seconds.

### Testing
- **Run all tests**: `dotnet test EFCore.Databricks.sln --no-build -c Release` -- takes ~3 seconds. NEVER CANCEL. Set timeout to 30+ seconds.
- **Run specific test**: `dotnet test EFCore.Databricks.sln --no-build -c Release --filter "FullyQualifiedName~TestName"`
- **CRITICAL**: Tests are EXPECTED to fail currently - this is by design as the project follows test-driven development. Tests exist before implementation.
- **Test failure pattern**: Most tests fail with "Unable to resolve service for type 'Microsoft.EntityFrameworkCore.Diagnostics.LoggingDefinitions'" indicating incomplete EF Core service registration.

### Project Structure
- **Solution**: `EFCore.Databricks.sln` - main solution file
- **Main library**: `src/EFCore.Databricks/` - contains the data provider implementation  
- **Tests**: `tests/EFCore.Databricks.Tests/` - xUnit tests with FakeItEasy mocking
- **Specifications**: `specs/001-efcore-databricks-read/` - comprehensive design docs
- **Scripts**: `scripts/` - project management shell scripts (`common.sh`, `create-new-feature.sh`, etc.)
- **Samples**: `samples/GraphQL/` - placeholder for future GraphQL sample (currently only README.md)
- **Templates**: `templates/` - document templates for specs and plans

### Key Implementation Files
- `src/EFCore.Databricks/Extensions/DatabricksDbContextOptionsExtensions.cs` - public API entry point
- `src/EFCore.Databricks/Infrastructure/DatabricksOptionsExtension.cs` - EF Core options extension
- `src/EFCore.Databricks/Infrastructure/DatabricksQuerySqlGenerator.cs` - SQL generation
- `src/EFCore.Databricks/Infrastructure/DatabricksRelationalConnection.cs` - ODBC connection wrapper

### Dependencies and Package Management
- **Core dependencies**: Microsoft.EntityFrameworkCore.Relational 9.0.8, System.Data.Odbc 9.0.0
- **Test dependencies**: xunit 2.5.3, FakeItEasy 9.3.0, Microsoft.NET.Test.Sdk 17.8.0
- **Always run `dotnet restore` before building after any package changes**

## Validation

### Build Validation
- **Always build and test after making changes**: Build should complete without warnings or errors.
- **Expected test state**: Tests failing due to incomplete implementation is NORMAL and EXPECTED.
- **Build config**: Uses `TreatWarningsAsErrors=true` - any warnings will fail the build.
- **Language features**: Uses `LangVersion=latest` and `Nullable=enable`.

### Code Quality
- **No dedicated linters**: The project relies on .NET compiler warnings and built-in analyzers.
- **CI validation**: `.github/workflows/ci.yml` runs restore → build → test pipeline.
- **Always ensure your changes don't introduce new compiler warnings**.

### Manual Testing Scenarios
- **Current limitation**: Cannot test end-to-end functionality yet as the provider is incomplete.
- **When implementation is complete**: Test basic LINQ queries like `ctx.Customers.Select(c => c.Name).OrderBy(c => c).Take(5).ToQueryString()`.
- **Integration tests**: Will require actual Databricks credentials via environment variables.

## Common Tasks

### Working with Tests
- **Test structure**: Tests are organized under `tests/EFCore.Databricks.Tests/Contract/`
- **Contract tests**: Verify SQL generation without requiring actual database connection
- **Test pattern**: Create failing tests first, then implement to make them pass
- **Key test files**:
  - `SelectProjectionTests.cs` - basic SELECT with projection
  - `SelectWhereTests.cs` - WHERE clauses with parameters
  - `SelectGroupByAggregateTests.cs` - GROUP BY with aggregates
  - `InnerJoinTests.cs` - INNER JOIN translation

### Understanding the Specifications
- **Always read**: `specs/001-efcore-databricks-read/spec.md` for feature requirements
- **Implementation guidance**: `specs/001-efcore-databricks-read/plan.md` and `tasks.md`
- **Key constraints**: Read-only provider, supports SELECT/WHERE/ORDER BY/GROUP BY/INNER JOIN/LIMIT
- **Unsupported features**: Will throw NotSupportedException for LEFT JOIN, OFFSET, window functions, SaveChanges

### Debugging Build Issues
- **Service registration errors**: Most test failures indicate missing EF Core service registrations in `DatabricksOptionsExtension.ApplyServices()`.
- **Connection string issues**: Provider expects connection string resolution from environment variables or explicit parameters.
- **ODBC dependency**: The provider uses `System.Data.Odbc.OdbcConnection` - no need to install separate ODBC drivers for building/testing.

## Environment and Configuration

### Environment Variables (for integration testing)
- `DATABRICKS_CONNECTION_STRING` - full ODBC connection string
- `DATABRICKS_DSN` - DSN name (requires DATABRICKS_TOKEN)  
- `DATABRICKS_TOKEN` - authentication token

### Development Workflow
1. **Read specifications first** - understand requirements before coding
2. **Write/update tests** - ensure tests exist for new functionality
3. **Build and verify** - `dotnet build` should succeed
4. **Run tests** - failures are expected until implementation is complete
5. **Implement incrementally** - make small changes and test frequently

### File Contents (frequently referenced)

#### Repository Root
```
.git/
.github/
.gitignore
Directory.Build.props
EFCore.Databricks.sln
README.md
memory/
samples/
scripts/
specs/
src/
templates/
tests/
```

#### EFCore.Databricks.csproj Key Settings
```xml
<TargetFramework>net8.0</TargetFramework>
<ImplicitUsings>enable</ImplicitUsings>
<LangVersion>latest</LangVersion>
<Nullable>enable</Nullable>
<TreatWarningsAsErrors>true</TreatWarningsAsErrors>
```

#### Package References
- Microsoft.EntityFrameworkCore.Relational: 8.0.8
- Microsoft.Extensions.Logging.Abstractions: 8.0.0  
- System.Data.Odbc: 8.0.0

### Quick Verification Commands
- `dotnet --version` - verify .NET 8 is available (should show 8.0.x)
- `dotnet restore EFCore.Databricks.sln` - restore packages (~20s first time, <2s subsequent)
- `dotnet build EFCore.Databricks.sln --no-restore -c Release` - build (~8s)  
- `dotnet test EFCore.Databricks.sln --no-build -c Release` - run tests (~3s, expected to fail)
- `dotnet clean EFCore.Databricks.sln` - clean build artifacts (~1s)
- `dotnet list src/EFCore.Databricks/EFCore.Databricks.csproj package` - show package dependencies

### Complete Build Cycle (from clean state)
```bash
dotnet clean EFCore.Databricks.sln && \
dotnet restore EFCore.Databricks.sln && \
dotnet build EFCore.Databricks.sln --no-restore -c Release
```
**Total time**: ~22 seconds first run, ~10 seconds subsequent runs. NEVER CANCEL. Set timeout to 120+ seconds.

## Debugging and Troubleshooting

### Common Issues
- **Service registration errors**: Most common test failure pattern indicates missing EF Core services in `DatabricksOptionsExtension.ApplyServices()`.
- **Connection resolution errors**: Provider expects connection string via environment variables or explicit configuration.
- **Missing ODBC drivers**: Tests don't require actual ODBC connectivity - they test SQL generation only.

### Development Status Indicators
- **Expected test state**: All tests failing with DI service resolution errors = normal development state
- **Successful implementation indicators**: Tests should begin passing as services are properly registered
- **Ready for integration testing**: Contract tests pass AND actual ODBC connection can be established

### Working with Specifications
- **Complete specification**: `specs/001-efcore-databricks-read/spec.md` contains full requirements
- **Implementation plan**: `specs/001-efcore-databricks-read/plan.md` shows technical approach  
- **Task breakdown**: `specs/001-efcore-databricks-read/tasks.md` shows step-by-step implementation plan
- **Design decisions**: `specs/001-efcore-databricks-read/research.md` documents key decisions

Remember: This is an early-stage project following test-driven development. Test failures are expected and normal until the implementation catches up with the test specifications.
