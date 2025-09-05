# Copilot Instructions for Databricks EF Core Provider

## Repository Overview
This repository contains an Entity Framework Core relational provider (EFCore.Databricks) for read-only LINQ queries against Databricks SQL via the Simba ODBC driver (System.Data.Odbc). 

## Technology Stack
- **.NET 8/9**: This project targets .NET 8 with forward compatibility for .NET 9 for the latest performance and language features
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
- Use .NET 8/9 features where beneficial for performance or code clarity
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