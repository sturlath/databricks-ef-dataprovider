# Contract: Logging Events (v0.1.0)

| EventId | Category | Level | Payload Keys | Description |
|---------|----------|-------|--------------|-------------|
| 1000 | EFCore.Databricks.Connection | Information | action(connect/open/close), elapsedMs, success | Connection lifecycle events (optional) |
| 2000 | EFCore.Databricks.Sql | Information | sql, parameterCount, elapsedMs, success | Executed command summary (parameters redacted) |
| 2001 | EFCore.Databricks.Sql | Debug | paramOrder | Order of positional parameters (for troubleshooting) |
| 3000 | EFCore.Databricks.TypeMapping | Debug | clrType, dbType | Type mapping resolution |
| 4000 | EFCore.Databricks.Unsupported | Warning | feature, reason | Attempted unsupported feature (thrown after logging) |

Redaction Rule: Any token or secret-like value replaced with `[REDACTED]`.
