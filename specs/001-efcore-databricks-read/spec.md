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
   → Mark with [NEEDS CLARIFICATION: specific question]
4. Fill User Scenarios & Testing section
   → If no clear user flow: ERROR "Cannot determine user scenarios"
5. Generate Functional Requirements
   → Each requirement must be testable
   → Mark ambiguous requirements
6. Identify Key Entities (if data involved)
7. Run Review Checklist
   → If any [NEEDS CLARIFICATION]: WARN "Spec has uncertainties"
   → If implementation details found: ERROR "Remove tech details"
8. Return: SUCCESS (spec ready for planning)
```

---

## ⚡ Quick Guidelines
- ✅ Focus on WHAT users need and WHY
- ❌ Avoid HOW to implement (no tech stack, APIs, code structure) [NEEDS CLARIFICATION: This feature request is inherently technical—what level of implementation detail is acceptable in this org's specs?]
- 👥 Written for business stakeholders, not developers

### Section Requirements
- **Mandatory sections**: Must be completed for every feature
- **Optional sections**: Include only when relevant to the feature
- When a section doesn't apply, remove it entirely (don't leave as "N/A")

### For AI Generation
When creating this spec from a user prompt:
1. **Mark all ambiguities**: Use [NEEDS CLARIFICATION: specific question] for any assumption you'd need to make
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
6. **Given** a query shape using a LEFT JOIN (projection requiring optional related data), **When** executed, **Then** (if left join support is included initially) valid `LEFT JOIN` SQL is produced; otherwise a documented limitation is surfaced. [NEEDS CLARIFICATION: Is LEFT JOIN required for initial release or can it slip to 0.2.0?]
7. **Given** a query employing an unsupported construct (e.g., window function, OFFSET, CROSS JOIN), **When** executed, **Then** a `NotSupportedException("Not implemented for now (read-only provider)")` is surfaced before executing against Databricks.
8. **Given** logging enabled at Information level, **When** executing a query, **Then** a log entry includes redacted parameter placeholders and execution duration.

### Edge Cases
- Query with zero results returns empty enumerable without error.
- Large numeric precision (decimal up to 38,18) is preserved in materialized results.
- Parameter ordering: Named LINQ parameters reordered internally still map to correct positional `?` order in generated SQL.
- Environment variables partially set (e.g., DSN but missing token) produce clear configuration error. [NEEDS CLARIFICATION: Expected precedence and mandatory fields when using DSN vs full connection string vs token?]
- Concurrent queries from multiple contexts do not leak parameters across commands.
- Guid and DateTimeOffset formatting produce valid Databricks-compatible literals when inlined (if ever) or bindings.

## Requirements *(mandatory)*

### Functional Requirements
- **FR-001**: The system MUST allow configuring a DbContext with a Databricks read-only provider via `UseDatabricks(...)` using either a connection string, a DSN name, or environment variables. [NEEDS CLARIFICATION: Priority order among DATABRICKS_CONNECTION_STRING vs DATABRICKS_DSN?]
- **FR-002**: The system MUST default all queries to no tracking behavior equivalent to `AsNoTracking()`.
- **FR-003**: The system MUST translate supported LINQ expressions (projection, filtering, ordering, grouping, aggregates, inner joins) to Databricks SQL using positional `?` parameters.
- **FR-004**: The system MUST support `LIMIT n` translation for `Take(n)` without generating unsupported `OFFSET` clauses.
- **FR-005**: The system MUST throw `NotSupportedException` with message "Not implemented for now (read-only provider)" for any attempt at data modification (insert/update/delete) or migrations APIs.
- **FR-006**: The system MUST throw the same NotSupportedException for unsupported query constructs (window functions, OFFSET, CROSS/RIGHT/FULL joins, UDFs).
- **FR-007**: The system MUST support LEFT JOIN translation if included in initial scope; otherwise queries requiring LEFT JOIN MUST surface a clear limitation message. [NEEDS CLARIFICATION: Confirm inclusion in v0.1.0]
- **FR-008**: The system MUST map EF Core parameter names to ODBC positional placeholders preserving correct ordinal binding.
- **FR-009**: The system MUST provide type mappings for: string, bool, int, long, double, decimal(38,18), DateTime, DateTimeOffset, Guid, byte[].
- **FR-010**: The system MUST implement logging that redacts parameter values while recording execution time and SQL text.
- **FR-011**: The system MUST allow optional integration tests to run only when required Databricks environment secrets are present.
- **FR-012**: The system MUST version the package starting at 0.1.0 using SemVer.
- **FR-013**: The system MUST provide documentation listing a supported LINQ → SQL translation matrix.
- **FR-014**: The system MUST fail fast with an explanatory error when configuration is incomplete or ambiguous.
- **FR-015**: The system MUST ensure thread-safe generation of unique parameter names/ordinals under concurrent query compilation.
- **FR-016**: The system MUST produce deterministic SQL text ordering for stable test assertions.
- **FR-017**: The system MUST redact secrets (tokens) from any exception or log output. [NEEDS CLARIFICATION: Standard redaction format, e.g., `***` vs `[REDACTED]`?]

### Key Entities *(include if feature involves data)*
- **Provider Configuration**: Conceptual representation of connection inputs (DSN, connection string, token). Attributes: input type, resolved final connection string, validation state.
- **Query Translation Unit**: Represents a parsed LINQ expression to be turned into SQL; attributes: projection list, predicates, joins, grouping, aggregates, limit.
- **Type Mapping Descriptor**: Represents logical CLR type to Databricks/ODBC type affinity and precision/scale where relevant.
- **Execution Log Entry**: Represents metadata about a completed query execution (SQL text, duration, parameter placeholders, success/failure flag).

---

## Review & Acceptance Checklist
*GATE: Automated checks run during main() execution*

### Content Quality
- [ ] No implementation details (languages, frameworks, APIs)  [NEEDS CLARIFICATION: This spec includes technical EF Core concepts—acceptable exception?]
- [x] Focused on user value and business needs (read-only querying via familiar abstraction)
- [ ] Written for non-technical stakeholders  [NEEDS CLARIFICATION: Audience seems mixed technical]
- [x] All mandatory sections completed

### Requirement Completeness
- [ ] No [NEEDS CLARIFICATION] markers remain
- [x] Requirements are testable and unambiguous (aside from marked clarifications)
- [ ] Success criteria are measurable  [NEEDS CLARIFICATION: Need performance target? e.g., translation latency?]
- [x] Scope is clearly bounded
- [ ] Dependencies and assumptions identified  [NEEDS CLARIFICATION: Enumerate external dependencies formally]

---

## Execution Status
*Updated by main() during processing*

- [x] User description parsed
- [x] Key concepts extracted
- [x] Ambiguities marked
- [x] User scenarios defined
- [x] Requirements generated
- [x] Entities identified
- [ ] Review checklist passed

---
