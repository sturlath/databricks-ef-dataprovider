# Databricks EF Core Provider Constitution

## Core Principles
### I. Provider‑First, Minimal Surface (NON‑NEGOTIABLE)

Build a single-purpose library (EFCore.Databricks, ADO.NET over ODBC) with the thinnest possible EF Core integration to enable read‑only LINQ queries. No migrations, no DDL, no update pipeline. Any non‑query APIs must throw NotSupportedException with the message: "Not implemented for now (read-only provider)". 

### II. Read‑Only Invariants

Only the query pipeline is supported. SaveChanges*, change tracking side effects, value generation, and any DML (INSERT/UPDATE/DELETE/MERGE) are out of scope. Provider must always use AsNoTracking() semantics; if absent, it may inject it transparently.

### III. Standards‑Driven SQL (Spark/Databricks Subset)

Target the lowest‑risk, broadly supported SQL subset:

- SELECT … FROM … with projections, WHERE, ORDER BY, GROUP BY, aggregates (COUNT, MIN, MAX, SUM, AVG).
- Joins: INNER JOIN (required), LEFT JOIN (recommended). No cross/outer/full joins initially.
- Pagination: LIMIT n. No OFFSET in v0; prefer keyset pagination (caller responsibility).
- Identifiers: provider avoids quoting by default; caller must supply safe identifiers. A future DialectOptions may enable backtick quoting if required.
- Parameters: positional ? via ODBC; the provider maps EF Core named parameters to positionals in emitted commands.

### IV. EF Core Compliance, But Only What We Use

Implement only the services required to translate basic LINQ → SQL and execute via System.Data.Odbc:
- Relational connection/command abstractions, type mapping for core primitives, SQL generation helper, query SQL generator factory, parameter name generator, and value converters for minimal types.
- If EF Core asks for unimplemented services (e.g., migrations), throw NotSupportedException as per Principle I.

### V. Test‑First with xUnit + FakeItEasy (NON‑NEGOTIABLE)

All features start as tests: contract tests (LINQ → expected SQL), unit tests for generators/mappings, and opt‑in integration tests behind secrets. Use xUnit + FakeItEasy only. Red‑Green‑Refactor is mandatory. Tests describe supported LINQ shapes explicitly; unsupported shapes must have failing/ignored tests documenting deferrals. 


### VI. Integration Testing (Real Databricks, Optional in CI)

A minimal end‑to‑end suite runs against a Databricks SQL Warehouse only when secrets are present (DSN or CSTR + token). These tests:

- Verify connectivity, parameter binding, basic projections/filters/joins/grouping.
- Are skipped by default in PR CI unless DATABRICKS_* secrets exist.
- Must never log tokens or query data.

### VII. Observability & Safety

- Structured logging (Microsoft.Extensions.Logging) with event IDs per component (Connection, SqlGen, TypeMap).
- Log the generated SQL with parameters redacted; never log secrets.
- Optional simple timing around command execution; no heavy instrumentation in v0.

### VIII. Versioning & Stability

- Semantic Versioning. Pre‑1.0: 0.y.z. First release is 0.1.0.
- “Breaking” within 0.y.z is allowed but documented. After 1.0, break only on MAJOR.
- EF Core pin: start with EF Core 9.x; evaluate EF Core 10 when stable.

### IX. Simplicity Over Features (YAGNI)

Prefer correctness + predictability to breadth. If a LINQ pattern expands SQL complexity (subqueries/windowing), it is out of scope for v0 unless trivial. Keep allocations low; use streaming DbDataReader.

### Technical Constraints & Scope
- Frameworks: net9.0 (required), net10.0 (when available).
- EF Core: Microsoft.EntityFrameworkCore.Relational 9.x.
- ADO.NET: System.Data.Odbc; Simba Databricks ODBC driver is an external prerequisite.
- Hot Chocolate: Provider must work behind a typical DbContext used by GraphQL resolvers; guidance: always query with AsNoTracking() and keyset pagination at the GraphQL layer.
- Type Mapping (v0): string, bool, int, long, double, decimal(38,18) (rounding via converter if needed), DateTime, DateTimeOffset, Guid, byte[].
- Unsupported in v0: migrations, DDL, DML, sequences, batching of multiple statements, UDFs, window functions, CROSS/RIGHT/FULL joins, complex navigation translation, table‑valued parameters, OFFSET pagination.
- Security: Only parameterized commands. No concatenated SQL. Connection info from DSN or connection string via env‑backed config (DATABRICKS_DSN or DATABRICKS_CONNECTION_STRING, DATABRICKS_TOKEN).
- Performance: Aim for parity with direct ODBC for simple queries (≤10% overhead in microbenchmarks for projection/filters).

### Development Workflow & Quality Gates
1. Planning
  - Open a Spec‑Kit spec referencing the LINQ shapes to support; include success/failure examples.
  - Update templates/checklists impacted by the change. 
2. Tests First
  -Author xUnit tests using FakeItEasy for services (SQL generator, type mappings, parameterization).
  - Contract tests assert exact SQL text (stable normalization rules) and parameter order.
3. Implementation
  - Implement only the services required to pass the new tests.
  - Any not‑yet‑supported EF Core interfaces must throw NotSupportedException with the standard message.
4. Review
  - PR must include: scope note, affected LINQ patterns, new/changed tests, doc snippet (README “Supported Queries” table).
  - CI gates: formatting/Analyzers as warnings‑as‑errors; unit tests required; integration tests optional/skipped without secrets.
5. Docs
  - Keep a living “Supported LINQ → SQL” matrix with examples.
  - Document environment variables and DSN setup for local runs.
6. Release
  - Tag v0.y.z. Update change log with supported shapes added.
  - If behavior changes for an existing shape, call it out as “Potentially Breaking (0.y.z)”.

### Governance
 - This constitution supersedes other practices for this repo. Amendments require:
   1) rationale, 2) test impact review, 3) migration notes in README, and 4) synchronization of Spec‑Kit templates per the Constitution Update Checklist. 
 - All PRs must verify: read‑only invariants preserved, parameterization enforced, logging redaction intact.
 - Use this constitution alongside the Spec‑Kit template structure as the canonical project contract. 

Version: 0.1.0 | Ratified: 2025‑09‑05 | Last Amended: 2025‑09‑05

Opinionated notes: Keyset pagination at the GraphQL layer is the right call; do not attempt OFFSET emulation in v0. Keep the SQL generator conservative—only shapes you can normalize and test deterministically.