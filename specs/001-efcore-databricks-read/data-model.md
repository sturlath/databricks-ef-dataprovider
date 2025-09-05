# Data Model (Phase 1)

The provider itself does not define persistent domain entities; instead it exposes EF Core infrastructure services. Conceptual internal entities used for reasoning & tests:

## ProviderConfiguration
Fields:
- SourceType (Enum: Explicit, ConnectionStringEnv, DsnEnv)
- RawInput (string)
- Dsn (string?)
- ConnectionString (string)
- Token (string?)
- ValidationErrors (IReadOnlyList<string>)

Rules:
- Must resolve to a valid ODBC connection string.
- Token required if not embedded and DSN lacks auth.

## TranslationRequest
Fields:
- ExpressionTree (System.Linq.Expressions.Expression)
- RequestedOperations (Flags: Projection, Filter, OrderBy, GroupBy, Aggregate, Join, Limit)
- ParameterBindings (List<ParameterBinding>)

## ParameterBinding
Fields:
- Name (string)
- Ordinal (int)
- ClrType (Type)

Constraints:
- Ordinals strictly increasing from first encounter.

## GeneratedSql
Fields:
- CommandText (string)
- ParameterCount (int)
- OrderedParameters (IReadOnlyList<ParameterBinding>)
- Diagnostics (KeyValue pairs: timing expectations, feature flags)

## ExecutionLogEntry
Fields:
- Sql (string)
- ElapsedMs (double)
- Success (bool)
- ParameterCount (int)
- ExceptionType (string?)

Relationship Notes:
- TranslationRequest → GeneratedSql (1:1 per compilation)
- GeneratedSql → ParameterBinding (1:N)
- ExecutionLogEntry references GeneratedSql but stores flattened data only.

-- End Phase 1 Data Model --
