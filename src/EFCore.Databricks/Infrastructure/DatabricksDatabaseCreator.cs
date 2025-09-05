using Microsoft.EntityFrameworkCore.Storage;

namespace EFCore.Databricks.Infrastructure
{
    /// <summary>
    /// Databricks read-only database creator (no DDL operations supported).
    /// </summary>
    public sealed class DatabricksDatabaseCreator(RelationalDatabaseCreatorDependencies dependencies) 
        : RelationalDatabaseCreator(dependencies)
    {
        public override bool CanConnect()
        {
            return Dependencies.Connection.DbConnection != null;
        }

        public override async Task<bool> CanConnectAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(CanConnect());
        }

        public override void Create()
        {
            throw new NotSupportedException("Database creation is not supported by the Databricks provider (read-only).");
        }

        public override Task CreateAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Database creation is not supported by the Databricks provider (read-only).");
        }

        public override void Delete()
        {
            throw new NotSupportedException("Database deletion is not supported by the Databricks provider (read-only).");
        }

        public override Task DeleteAsync(CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException("Database deletion is not supported by the Databricks provider (read-only).");
        }

        public override bool Exists()
        {
            // For a read-only provider, we assume the database/warehouse exists
            return true;
        }

        public override async Task<bool> ExistsAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(Exists());
        }

        public override bool HasTables()
        {
            // For a read-only provider, we assume tables exist
            return true;
        }

        public override async Task<bool> HasTablesAsync(CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(HasTables());
        }
    }
}