# Feature Specification: EFCore.Databricks Read-Only Provider (Initial Capability Scope)

**Feature Branch**: `001-efcore-databricks-read`  
**Created**: 2025-09-05  
**Status**: Draft  
**Input**: User description: "Build an EF Core provider 'EFCore.Databricks' that enables read‑only LINQ queries against Databricks SQL via Simba ODBC (System.Data.Odbc). Target net9.0 (add net10.0 when available) and EF Core 9/10; expose UseDatabricks(DbContextOptionsBuilder, string|DSN|env) and read DATABRICKS_DSN|DATABRICKS_CONNECTION_STRING|DATABRICKS_TOKEN. Scope: SELECT with projection, WHERE, ORDER BY, GROUP BY, aggregates; INNER JOIN (required), LEFT JOIN (nice‑to‑have); LIMIT n pagination; map EF named params → ODBC positional ?; default AsNoTracking(). Out of scope: migrations/DDL/DML/SaveChanges, window functions, OFFSET, CROSS/RIGHT/FULL joins, UDFs—these throw NotSupportedException("Not implemented for now (read-only provider)"). Implement minimal services: provider options/extension, RelationalConnection (ODBC), SqlGenerationHelper, QuerySqlGeneratorFactory/QuerySqlGenerator, RelationalTypeMappingSource for primitives (string,bool,int,long,double,decimal(38,18),DateTime,DateTimeOffset,Guid,byte[]), parameter name generator. Logging: Microsoft.Extensions.Logging with redacted parameters and basic execution timing. Testing: xUnit + FakeItEasy only—contract tests assert LINQ→SQL text + param order; unit tests for mappings/generator; optional integration tests gated by DATABRICKS_* secrets. Deliverables: src/EFCore.Databricks, tests/EFCore.Databricks.Tests, samples/GraphQL (HotChocolate resolver with keyset pagination + AsNoTracking()), README with “Supported LINQ→SQL matrix” + setup. Versioning: start at 0.1.0 (SemVer); CI skips integration tests without secrets." 

## Execution Flow (main)
```
1. Parse user description from Input
   → If empty: ERROR "No feature description provided"
2. Extract key concepts from description
   → Identify: actors, actions, data, constraints
3. For each unclear aspect:
   → (Finalized) No open clarification questions remain; future questions must be added explicitly before implementation changes.
4. Fill User Scenarios & Testing section
   → If no clear user flow: ERROR "Cannot determine user scenarios"
5. Generate Functional Requirements
   → Each requirement must be testable
   → Mark ambiguous requirements
6. Identify Key Entities (if data involved)
7. Run Review Checklist
   → If any unresolved questions exist: WARN "Spec has uncertainties"
   → If implementation details found: ERROR "Remove tech details"
8. Return: SUCCESS (spec ready for planning)
```

---

## ⚡ Quick Guidelines
- ✅ Focus on WHAT users need and WHY
- ❌ Keep HOW details minimal; naming EF Core concepts (DbContext, LINQ, joins) is ACCEPTABLE for this product-facing technical spec, but exclude low-level class design / method bodies.
- 👥 Primary audience: technical stakeholders (platform engineers & senior developers) plus product owners needing scope clarity.

### Section Requirements
- **Mandatory sections**: Must be completed for every feature
- **Optional sections**: Include only when relevant to the feature
- When a section doesn't apply, remove it entirely (don't leave as "N/A")

### For AI Generation
When creating this spec from a user prompt:
1. **Mark all ambiguities**: (Guideline retained for future specs; none currently outstanding.)
2. **Don't guess**: If the prompt doesn't specify something (e.g., "login system" without auth method), mark it
3. **Think like a tester**: Every vague requirement should fail the "testable and unambiguous" checklist item
4. **Common underspecified areas**:
   - User types and permissions
   - Data retention/deletion policies  
   - Performance targets and scale
   - Error handling behaviors
   - Integration requirements
   - Security/compliance needs

---

## User Scenarios & Testing *(mandatory)*

### Primary User Story
Data platform / application developers need to execute read-only analytical or reporting queries using familiar Entity Framework Core LINQ syntax against Databricks SQL endpoints without writing raw SQL, enabling faster adoption, consistent query composition, and integration with existing EF-based abstraction layers while explicitly preventing unintended data modification operations.

### Acceptance Scenarios
1. **Given** an application configured with UseDatabricks and environment variable `DATABRICKS_CONNECTION_STRING` set, **When** a developer executes a LINQ query with filters and projections, **Then** the provider translates it into valid Databricks SQL using positional `?` parameters and returns materialized objects in no-tracking mode.
2. **Given** a LINQ query performing an aggregate (e.g., `group by` with `Count()`), **When** executed, **Then** the generated SQL includes correct `GROUP BY` clauses and aggregate functions and executes successfully.
3. **Given** a query joining two entity sets with an inner join via navigation or join clause, **When** executed, **Then** the provider emits a valid `INNER JOIN` and returns correct result ordering when `OrderBy` is applied.
4. **Given** a query requesting the first N rows via `Take(n)`, **When** executed, **Then** the SQL contains `LIMIT n` and no unsupported `OFFSET` fragment.
5. **Given** an attempt to call `SaveChanges()` or to add/update/delete tracked entities, **When** invoked, **Then** a `NotSupportedException` stating read-only limitation is thrown.
6. **Given** a query shape requiring a LEFT join (optional related data), **When** executed under v0.1.0, **Then** a `NotSupportedException` clearly states LEFT JOIN is scheduled for v0.2.0 rather than producing incorrect SQL.
7. **Given** a query employing an unsupported construct (e.g., window function, OFFSET, CROSS/RIGHT/FULL join, UDF), **When** executed, **Then** a `NotSupportedException("Not implemented for now (read-only provider)")` is surfaced before executing against Databricks.
8. **Given** logging enabled at Information level, **When** executing a query, **Then** a log entry includes redacted parameter placeholders and execution duration.

### Edge Cases
- Query with zero results returns empty enumerable without error.
- Large numeric precision (decimal up to 38,18) is preserved in materialized results.
- Parameter ordering: Named LINQ parameters reordered internally still map to correct positional `?` order in generated SQL.
- Configuration precedence (decided): Explicit method argument > `DATABRICKS_CONNECTION_STRING` > `DATABRICKS_DSN` (with mandatory `DATABRICKS_TOKEN` if token not embedded). Missing required token yields clear error: "Databricks token not provided via connection string or DATABRICKS_TOKEN." 
- DSN mode: If DSN chosen, final connection string is `DSN=<value>;` plus token if not already resolvable through DSN.
- Concurrent queries from multiple contexts do not leak parameters across commands.
- Guid and DateTimeOffset formatting produce valid Databricks-compatible bound parameter representations; no literal inlining for sensitive values.

## Requirements *(mandatory)*

### Functional Requirements
- **FR-001**: The system MUST allow configuring a DbContext with a Databricks read-only provider via `UseDatabricks(...)` using either a connection string, a DSN name, or environment variables with precedence: explicit argument > `DATABRICKS_CONNECTION_STRING` > `DATABRICKS_DSN` (+ `DATABRICKS_TOKEN`).
- **FR-002**: The system MUST default all queries to no tracking behavior equivalent to `AsNoTracking()`.
- **FR-003**: The system MUST translate supported LINQ expressions (projection, filtering, ordering, grouping, aggregates, inner joins) to Databricks SQL using positional `?` parameters.
- **FR-004**: The system MUST support `LIMIT n` translation for `Take(n)` without generating unsupported `OFFSET` clauses.
- **FR-005**: The system MUST throw `NotSupportedException` with message "Not implemented for now (read-only provider)" for any attempt at data modification (insert/update/delete) or migrations APIs.
- **FR-006**: The system MUST throw the same NotSupportedException for unsupported query constructs (window functions, OFFSET, CROSS/RIGHT/FULL joins, UDFs).
- **FR-007**: The system MUST NOT support LEFT JOIN in v0.1.0; attempting a LEFT JOIN MUST raise a NotSupportedException referencing roadmap (planned for v0.2.0).
- **FR-008**: The system MUST map EF Core parameter names to ODBC positional placeholders preserving correct ordinal binding.
- **FR-009**: The system MUST provide type mappings for: string, bool, int, long, double, decimal(38,18), DateTime, DateTimeOffset, Guid, byte[].
- **FR-010**: The system MUST implement logging that redacts parameter values while recording execution time and SQL text.
- **FR-011**: The system MUST allow optional integration tests to run only when required Databricks environment secrets are present.
- **FR-012**: The system MUST version the package starting at 0.1.0 using SemVer.
- **FR-013**: The system MUST provide documentation listing a supported LINQ → SQL translation matrix.
- **FR-014**: The system MUST fail fast with an explanatory error when configuration is incomplete or ambiguous.
- **FR-015**: The system MUST ensure thread-safe generation of unique parameter names/ordinals under concurrent query compilation.
- **FR-016**: The system MUST produce deterministic SQL text ordering for stable test assertions.
- **FR-017**: The system MUST redact secrets (tokens) from any exception or log output using the literal replacement string `[REDACTED]` for the secret value.
- **FR-018**: 95% of simple translation operations (projection + filter + limit) MUST compile to SQL in <50 ms on a developer laptop (baseline test harness) for expressions under 25 nodes.
- **FR-019**: Logging MUST include elapsed time with millisecond precision and a correlation identifier when an ambient Activity/TraceId is present.

### Key Entities *(include if feature involves data)*
- **Provider Configuration**: Conceptual representation of connection inputs (DSN, connection string, token). Attributes: input type, resolved final connection string, validation state.
- **Query Translation Unit**: Represents a parsed LINQ expression to be turned into SQL; attributes: projection list, predicates, joins, grouping, aggregates, limit.
- **Type Mapping Descriptor**: Represents logical CLR type to Databricks/ODBC type affinity and precision/scale where relevant.
- **Execution Log Entry**: Represents metadata about a completed query execution (SQL text, duration, parameter placeholders, success/failure flag).

---

## Review & Acceptance Checklist
*GATE: Automated checks run during main() execution*

### Content Quality
- [x] Minimal implementation details (only necessary EF Core surface named)
- [x] Focused on user value and business needs
- [x] Appropriate for mixed technical/product audience
- [x] All mandatory sections completed

### Requirement Completeness
- [x] No clarification markers remain
- [x] Requirements are testable and unambiguous
- [x] Success criteria are measurable (see performance & determinism targets)
- [x] Scope is clearly bounded
- [x] Dependencies and assumptions identified

### Dependencies & Assumptions
- Databricks SQL endpoint accessible via Simba ODBC driver (installed externally).
- .NET 9 runtime available; future .NET 10 multi-targeting to be added without breaking changes.
- EF Core 9 baseline; forward compatibility validated with EF Core 10 previews before 1.0.
- Logging pipeline uses Microsoft.Extensions.Logging abstractions; consumers provide sinks.
- Environment secrets provided securely (not committed) for integration tests.
- No write permission required; security model relies on Databricks endpoint enforcing read-only principal.
- Performance target measured on representative developer hardware (>= 8 logical cores, SSD).

---

## Execution Status
*Updated by main() during processing*

- [x] User description parsed
- [x] Key concepts extracted
- [x] Ambiguities resolved
- [x] User scenarios defined
- [x] Requirements generated
- [x] Entities identified
- [x] Review checklist passed

---
