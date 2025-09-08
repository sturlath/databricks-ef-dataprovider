using System.Text;
using Microsoft.EntityFrameworkCore.Update;

namespace EFCore.Databricks.Infrastructure
{
    /// <summary>
    /// Update SQL generator for Databricks provider.
    /// Since this is a read-only provider, all methods throw NotSupportedException.
    /// </summary>
    public class DatabricksUpdateSqlGenerator : IUpdateSqlGenerator
    {
        public ResultSetMapping AppendBulkInsertOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyList<IReadOnlyModificationCommand> modificationCommands,
            int commandPosition)
        {
            throw new NotSupportedException("Databricks provider is read-only. Insert operations are not supported.");
        }

        public ResultSetMapping AppendDeleteOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition)
        {
            throw new NotSupportedException("Databricks provider is read-only. Delete operations are not supported.");
        }

        public ResultSetMapping AppendDeleteOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition,
            out bool requiresTransaction)
        {
            throw new NotSupportedException("Databricks provider is read-only. Delete operations are not supported.");
        }

        public ResultSetMapping AppendInsertOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition)
        {
            throw new NotSupportedException("Databricks provider is read-only. Insert operations are not supported.");
        }

        public ResultSetMapping AppendInsertOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition,
            out bool requiresTransaction)
        {
            throw new NotSupportedException("Databricks provider is read-only. Insert operations are not supported.");
        }

        public ResultSetMapping AppendUpdateOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition)
        {
            throw new NotSupportedException("Databricks provider is read-only. Update operations are not supported.");
        }

        public ResultSetMapping AppendUpdateOperation(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition,
            out bool requiresTransaction)
        {
            throw new NotSupportedException("Databricks provider is read-only. Update operations are not supported.");
        }

        public ResultSetMapping AppendStoredProcedureCall(
            StringBuilder commandStringBuilder,
            IReadOnlyModificationCommand command,
            int commandPosition,
            out bool requiresTransaction)
        {
            throw new NotSupportedException("Databricks provider is read-only. Stored procedure calls are not supported.");
        }

        public string GenerateNextSequenceValueOperation(string name, string? schema)
        {
            throw new NotSupportedException("Databricks provider is read-only. Sequence operations are not supported.");
        }

        public void AppendNextSequenceValueOperation(StringBuilder commandStringBuilder, string name, string? schema)
        {
            throw new NotSupportedException("Databricks provider is read-only. Sequence operations are not supported.");
        }

        public string GenerateObtainNextSequenceValueOperation(string name, string? schema)
        {
            throw new NotSupportedException("Databricks provider is read-only. Sequence operations are not supported.");
        }

        public void AppendObtainNextSequenceValueOperation(StringBuilder commandStringBuilder, string name, string? schema)
        {
            throw new NotSupportedException("Databricks provider is read-only. Sequence operations are not supported.");
        }

        public void AppendBatchHeader(StringBuilder commandStringBuilder)
        {
            // No-op for read-only provider
        }

        public void PrependEnsureAutocommit(StringBuilder commandStringBuilder)
        {
            // No-op for read-only provider
        }
    }
}