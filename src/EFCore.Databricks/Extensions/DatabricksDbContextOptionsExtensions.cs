using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides helpers for configuring a <see cref="DbContext"/> to use the
    /// Databricks provider.
    /// </summary>
    public static class DatabricksDbContextOptionsExtensions
    {
        /// <summary>
        /// Configures the context to use the Databricks provider. The connection
        /// string is resolved using the following precedence:
        /// explicit <paramref name="connectionString"/> parameter,
        /// environment variable <c>DATABRICKS_CONNECTION_STRING</c>, then
        /// <c>DATABRICKS_DSN</c> combined with <c>DATABRICKS_TOKEN</c>.
        /// </summary>
        public static DbContextOptionsBuilder UseDatabricks(
            this DbContextOptionsBuilder optionsBuilder,
            string? connectionString = null)
        {
            var resolved = ResolveConnectionString(connectionString);

            var extension = optionsBuilder.Options.FindExtension<DatabricksOptionsExtension>()
                ?? new DatabricksOptionsExtension();
            extension = extension.WithConnectionString(resolved);

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder).AddOrUpdateExtension(extension);

            optionsBuilder.ConfigureWarnings(w => w.Default(WarningBehavior.Throw));
            optionsBuilder.UseSqlite("Data Source=:memory:");
            optionsBuilder.ReplaceService<ISqlGenerationHelper, DatabricksSqlGenerationHelper>();
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            return optionsBuilder;
        }

        private static string ResolveConnectionString(string? provided)
        {
            if (!string.IsNullOrWhiteSpace(provided))
            {
                return provided;
            }

            var cs = Environment.GetEnvironmentVariable("DATABRICKS_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(cs))
            {
                return cs;
            }

            var dsn = Environment.GetEnvironmentVariable("DATABRICKS_DSN");
            if (!string.IsNullOrWhiteSpace(dsn))
            {
                var token = Environment.GetEnvironmentVariable("DATABRICKS_TOKEN");
                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new InvalidOperationException("Databricks token not provided via connection string or DATABRICKS_TOKEN.");
                }

                return $"DSN={dsn};UID=token;PWD={token}";
            }

            throw new InvalidOperationException("No Databricks connection information found.");
        }
    }
}
