# Implementation Plan: EFCore.Databricks Read-Only Provider

**Branch**: `001-efcore-databricks-read` | **Date**: 2025-09-05 | **Spec**: specs/001-efcore-databricks-read/spec.md
**Input**: Feature specification from `/specs/001-efcore-databricks-read/spec.md`

## Execution Flow (/plan command scope)
(Executed manually due to unavailable bash script environment)
1. Loaded feature spec ✔
2. Filled Technical Context ✔
3. Initial Constitution Check ✔ (no violations)
4. Phase 0 research created (research.md) ✔
5. Phase 1 artifacts created (data-model.md, contracts, quickstart.md) ✔
6. Post-Design Constitution Check ✔
7. Phase 2 planning approach documented below ✔
8. STOP (Tasks generation deferred)

## Summary
Provide a minimal, deterministic, read-only EF Core provider over System.Data.Odbc for Databricks SQL supporting core SELECT patterns (projection, filter, order, group+aggregate, inner join) and LIMIT, excluding LEFT JOIN (deferred), with strict read-only invariants and deterministic SQL generation.

## Technical Context
**Language/Version**: C# / .NET 9.0 (future multi-target .NET 10)  
**Primary Dependencies**: Microsoft.EntityFrameworkCore.Relational 9.x, System.Data.Odbc, Microsoft.Extensions.Logging.Abstractions, FakeItEasy (tests), xUnit, HotChocolate (sample)  
**Storage**: Databricks SQL via ODBC (external)  
**Testing**: xUnit + FakeItEasy (contract, unit), optional integration (env-gated)  
**Target Platform**: Cross-platform .NET (dev focus Windows/macOS/Linux)  
**Project Type**: Single library + tests + sample  
**Performance Goals**: 95% simple translations <50ms; provider overhead ≤10% vs raw ODBC for basic queries  
**Constraints**: Read-only; no migrations/DDL/DML; no OFFSET/window funcs; deterministic SQL; positional parameters  
**Scale/Scope**: Initial release limited to core query shapes; roadmap adds LEFT JOIN, broader translations

## Constitution Check
Simplicity: 1 library project (provider) + tests + sample (graphQL) – within limit. No unnecessary patterns (no repositories/UoW).  
Architecture: Single provider library; docs & samples separate.  
Testing: Plan enforces contract tests first. Integration optional/gated.  
Observability: Structured logging, redaction confirmed.  
Versioning: Start 0.1.0; SemVer pre-1 allowances.  
All constitutional principles satisfied without exceptions.

## Project Structure
src/EFCore.Databricks (provider)
 tests/EFCore.Databricks.Tests (unit + contract + integration folders)
 samples/GraphQL (HotChocolate demo using keyset pagination)

## Phase 0: Outline & Research
Completed (see research.md for RD-001..RD-010 decisions).

## Phase 1: Design & Contracts
Artifacts produced:
- data-model.md (internal conceptual entities)
- contracts/logging-events.md
- contracts/public-api.md
- quickstart.md
No external HTTP/GraphQL contracts required (provider library). Sample GraphQL schema deferred to implementation tasks.

## Phase 2: Task Planning Approach
Task Generation Strategy:
- Derive tasks from public-api, logging-events, type mapping list, and SQL generator capabilities.
- Contracts → contract test tasks (e.g., translation for WHERE, GROUP BY, LIMIT, INNER JOIN).
- Data model entities → implementation tasks (ProviderConfiguration, Parameter strategy, SqlGenerator, TypeMappingSource).
- Performance → micro-benchmark harness (optional post-0.1, mark as deferred task).
Ordering Strategy:
1. Skeleton solution + project files (csproj with analyzers, nullable, warnings as errors, SourceLink)
2. Public API extension + options object + argument/env resolution tests
3. Type mapping source + tests
4. Parameter name/ordinal generator + tests
5. SQL generator (SELECT baseline) + contract tests (projection, WHERE)
6. Add ORDER BY, LIMIT, GROUP BY + aggregates tests then implementation
7. Add INNER JOIN tests then implementation
8. NotSupported cases tests (LEFT JOIN, window, OFFSET) then exception guards
9. Logging instrumentation tests
10. Integration tests (gated) scaffolding
11. Sample GraphQL project skeleton
12. README supported matrix
13. Packaging (NuGet metadata, version 0.1.0) and CI workflow
Parallelizable ([P]): type mapping, parameter generator, logging event IDs doc maintenance once skeleton in place.
Estimated Output: ~28 tasks.

## Phase 3+: Future Implementation
Deferred to /tasks command.

## Complexity Tracking
| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| (none) | | |

## Progress Tracking
**Phase Status**:
- [x] Phase 0: Research complete (/plan command)
- [x] Phase 1: Design complete (/plan command)
- [x] Phase 2: Task planning complete (/plan command - describe approach only)
- [ ] Phase 3: Tasks generated (/tasks command)
- [ ] Phase 4: Implementation complete
- [ ] Phase 5: Validation passed

**Gate Status**:
- [x] Initial Constitution Check: PASS
- [x] Post-Design Constitution Check: PASS
- [x] All NEEDS CLARIFICATION resolved
- [x] Complexity deviations documented (none)

-- End /plan output --
