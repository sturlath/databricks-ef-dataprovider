# Tasks: EFCore.Databricks Read-Only Provider v0.1.0

**Input**: Design documents from `/specs/001-efcore-databricks-read/`
**Prerequisites**: plan.md (required), research.md, data-model.md, contracts/

## Execution Flow (generated)
Derived from plan → Setup, Tests (fail first), Core, Integration, Polish per constitution.

## Phase 3.1: Setup
- [ ] T001 Create solution & project structure: `EFCore.Databricks.sln`, `src/EFCore.Databricks/EFCore.Databricks.csproj`, `tests/EFCore.Databricks.Tests/EFCore.Databricks.Tests.csproj`, `samples/GraphQL/` skeleton.
- [ ] T002 Add package references (Relational 9.x, Logging.Abstractions) and build settings (Nullable enable, TreatWarningsAsErrors, SourceLink) in `src/EFCore.Databricks/EFCore.Databricks.csproj`.
- [ ] T003 [P] Add test project dependencies (xUnit, xUnit.runner.visualstudio, FakeItEasy) and directory layout: `tests/EFCore.Databricks.Tests/{Contract,Unit,Integration}`.
- [ ] T004 [P] Add CI workflow skeleton `.github/workflows/ci.yml` (build, test, conditional integration tests, pack).
- [ ] T005 [P] Add `Directory.Build.props` for shared analyzer rules & LangVersion.

## Phase 3.2: Tests First (Contract & Guard Tests) ⚠️ MUST FAIL INITIALLY
Contract tests assert SQL text + param order before implementation.
- [ ] T006 Create contract test `tests/EFCore.Databricks.Tests/Contract/SelectProjectionTests.cs` (simple projection).
- [ ] T007 [P] Create contract test `.../SelectWhereTests.cs` (WHERE with parameters and order of `?`).
- [ ] T008 [P] Create contract test `.../SelectGroupByAggregateTests.cs` (GROUP BY + COUNT/SUM).
- [ ] T009 [P] Create contract test `.../SelectOrderByLimitTests.cs` (ORDER BY + Take → LIMIT).
- [ ] T010 [P] Create contract test `.../InnerJoinTests.cs` (INNER JOIN translation).
- [ ] T011 [P] Create contract test `.../LeftJoinUnsupportedTests.cs` expecting NotSupportedException.
- [ ] T012 [P] Create contract test `.../UnsupportedWindowFunctionTests.cs` expecting NotSupportedException.
- [ ] T013 [P] Create contract test `.../UnsupportedOffsetTests.cs` expecting NotSupportedException.
- [ ] T014 Integration test skeleton `tests/EFCore.Databricks.Tests/Integration/BasicQueryTests.cs` (skip unless env present) verifying round-trip SELECT.
- [ ] T015 [P] Quickstart scenario test `tests/EFCore.Databricks.Tests/Contract/QuickstartScenarioTests.cs` (subset of quickstart steps).

## Phase 3.3: Core Implementation (Only after T006–T015 committed & failing)
- [ ] T016 Implement public API extension `UseDatabricks` in `src/EFCore.Databricks/Extensions/DatabricksDbContextOptionsExtensions.cs` with env precedence logic.
- [ ] T017 Add options class `DatabricksOptions` & configuration binder logic in `src/EFCore.Databricks/Infrastructure/DatabricksOptions.cs`.
- [ ] T018 Implement `ProviderConfiguration` internal model + validation utility in `src/EFCore.Databricks/Configuration/ProviderConfigurationFactory.cs`.
- [ ] T019 Implement RelationalTypeMappingSource subclass `DatabricksTypeMappingSource` for required CLR types.
- [ ] T020 [P] Implement parameter name/ordinal generator `SequentialParameterNameGenerator`.
- [ ] T021 Implement `DatabricksSqlGenerationHelper` (identifier handling, parameter placeholder `?`).
- [ ] T022 Implement basic `DatabricksQuerySqlGenerator` + factory supporting SELECT + WHERE + projection.
- [ ] T023 Extend SQL generator: ORDER BY + LIMIT.
- [ ] T024 Extend SQL generator: GROUP BY + aggregates.
- [ ] T025 Implement INNER JOIN translation.
- [ ] T026 Add NotSupported guards (LEFT JOIN, OFFSET, window functions) centralized helper.
- [ ] T027 Implement ODBC relational connection `DatabricksRelationalConnection` using `System.Data.Odbc`.
- [ ] T028 Implement command execution pipeline (CreateCommand, parameter binding positional) + logging.

## Phase 3.4: Integration & Observability
- [ ] T029 Add structured logging event IDs per `contracts/logging-events.md`.
- [ ] T030 [P] Add redaction utility `[REDACTED]` for tokens and integrate into logging.
- [ ] T031 [P] Add performance timing (Stopwatch) around execution path.
- [ ] T032 Integration test enablement: detect env & un-skip tests (validate parameter order & data retrieval).

## Phase 3.5: Polish & Documentation
- [ ] T033 Add README with Supported LINQ → SQL matrix and configuration instructions.
- [ ] T034 [P] Add XML doc comments for public API + generate `DocumentationFile` in csproj.
- [ ] T035 [P] Add CHANGELOG.md with unreleased 0.1.0 section.
- [ ] T036 [P] Add SourceLink + deterministic build verification (embed commit id) in csproj.
- [ ] T037 Add package metadata (Description, Tags, RepositoryUrl, License) in csproj.
- [ ] T038 Add benchmark stub (optional) `tests/EFCore.Databricks.Tests/Performance/TranslationBenchmarks.cs` (can be skipped until later) marking performance target.
- [ ] T039 Verify all NotSupported paths have tests & consistent message.
- [ ] T040 Final pass: ensure all tests green, update README matrix accordingly.

## Dependencies & Ordering
- Setup (T001–T005) precedes all tests.
- Contract tests (T006–T015) precede implementation tasks (T016+).
- SQL generator tasks build in layers: T022 → T023 → T024 → T025 → T026.
- Logging tasks depend on execution pipeline (T028) before T029–T031.
- Documentation tasks after core + integration (post T032) except README scaffold could start earlier (kept later for accuracy).

## Parallelizable [P] Justification
- T003/T004/T005 modify distinct files.
- Contract tests T007–T013, T015 each separate file.
- T020, T030, T031, T034–T036 in distinct files/nodes.

## Parallel Execution Example
```
# After T006 created initial contract test, run in parallel:
T007 SelectWhereTests.cs
T008 SelectGroupByAggregateTests.cs
T009 SelectOrderByLimitTests.cs
T010 InnerJoinTests.cs
T011 LeftJoinUnsupportedTests.cs
T012 UnsupportedWindowFunctionTests.cs
T013 UnsupportedOffsetTests.cs
T015 QuickstartScenarioTests.cs
```

## Validation Checklist
- [ ] All contract files have corresponding tests (logging-events/public-api represented via tests T016+ indirectly)
- [ ] All conceptual entities have tasks (ProviderConfiguration, ParameterBinding, etc.)
- [ ] Tests precede implementation
- [ ] Parallel tasks independent
- [ ] Each task lists concrete file path
- [ ] Unsupported features explicitly tested

-- End tasks.md --
