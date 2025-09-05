# Phase 0 Research: EFCore.Databricks Read-Only Provider

**Branch**: 001-efcore-databricks-read  
**Spec**: c:\Dev\databricks-ef-dataprovider\databricks-ef-dataprovider\specs\001-efcore-databricks-read\spec.md  
**Date**: 2025-09-05

## Research Decisions & Rationale

### RD-001 LEFT JOIN Deferral
- Decision: Exclude LEFT JOIN in v0.1.0.
- Rationale: Reduces initial SQL generator complexity; keeps normalization deterministic.
- Alternatives: Implement subset now – rejected (adds complex null propagation tests).

### RD-002 Configuration Precedence
- Decision: explicit method arg > DATABRICKS_CONNECTION_STRING > DATABRICKS_DSN (+ DATABRICKS_TOKEN).
- Rationale: Principle of least surprise; explicit call site always wins.
- Alternatives: DSN first – rejected (implicit + harder debugging).

### RD-003 Redaction Format
- Decision: Use literal `[REDACTED]` for any secret value.
- Rationale: Consistent with security logging norms; grep-able.
- Alternatives: `***` – less descriptive.

### RD-004 Performance Target
- Decision: 95% of simple translations (<25 expression nodes) compile <50ms.
- Rationale: User experience of interactive queries; aligns with constitution (≤10% overhead vs raw ODBC).
- Alternatives: Higher threshold (100ms) – rejected (sluggish dev feedback).

### RD-005 Deterministic SQL Ordering
- Decision: Emit projection, FROM, WHERE, GROUP BY, ORDER BY, LIMIT with consistent clause ordering and stable alias generation.
- Rationale: Test reliability and caching.

### RD-006 Logging Scope
- Decision: Single structured log per executed command at Information with: EventId, Category="EFCore.Databricks.Sql", SQL (params redacted), parameter count, elapsed ms, success/failure.
- Rationale: Minimal yet actionable observability.
- Alternatives: Per-clause debug logs – deferred to advanced diagnostics feature.

### RD-007 Type Mapping Strategy
- Decision: Map to closest Spark SQL/Databricks types through ODBC: string→STRING, bool→BOOLEAN, int→INT, long→BIGINT, double→DOUBLE, decimal(38,18)→DECIMAL(38,18), DateTime→TIMESTAMP (no TZ), DateTimeOffset→TIMESTAMP (store offset-adjusted UTC; convert), Guid→STRING (canonical 36-char), byte[]→BINARY.
- Rationale: Align with Databricks SQL capabilities.
- Risk: DateTimeOffset nuance (offset lost) documented in README.

### RD-008 Parameter Strategy
- Decision: Maintain ordered list during SQL generation; assign ordinal on encounter to produce positional `?`; keep mapping dictionary name→index for diagnostics only.
- Rationale: Guarantees order alignment; avoids rewrite later.

### RD-009 NotSupported Enforcement
- Decision: Central helper `ThrowReadOnly()` with canonical message.
- Rationale: Uniform messaging and easier test assertions.

### RD-010 Integration Test Gating
- Decision: Skip integration tests when any of required env vars absent (token + DSN or connection string). Use xUnit `Skip` reason.
- Rationale: Predictable CI behavior; no conditional build failures.

## Open Risks / Mitigations
- Risk: ODBC driver differences across developer machines.
  - Mitigation: Document required Simba driver version & DSN template.
- Risk: Decimal precision mismatch.
  - Mitigation: Add round-trip contract test with decimal extremes.
- Risk: DateTimeOffset semantics.
  - Mitigation: Document conversion + provide extension sample with explicit UTC usage.

## Out-of-Scope Confirmations
Matches spec & constitution: migrations, DDL, DML, OFFSET, window, complex joins.

## Unknowns Resolved
No remaining clarification markers; all design decisions captured above.

-- End Phase 0 Research (READY) --
