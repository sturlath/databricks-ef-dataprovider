using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EFCore.Databricks.Infrastructure
{
    /// <summary>
    /// EF Core options extension holding Databricks specific settings.
    /// </summary>
    public sealed class DatabricksOptionsExtension : RelationalOptionsExtension
    {
        private DbContextOptionsExtensionInfo? _info;

        public override string? ConnectionString { get; }

        public DatabricksOptionsExtension()
        {
        }

        public DatabricksOptionsExtension(string connectionString)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public override RelationalOptionsExtension WithConnectionString(string? connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Connection string cannot be null or whitespace.", nameof(connectionString));
            }
            return new DatabricksOptionsExtension(connectionString);
        }

        public override void ApplyServices(IServiceCollection services)
        {
            new EntityFrameworkRelationalServicesBuilder(services)
                .TryAddCoreServices();

            services.Replace(ServiceDescriptor.Singleton<ISqlGenerationHelper, DatabricksSqlGenerationHelper>());
            services.Replace(ServiceDescriptor.Singleton<IRelationalTypeMappingSource, DatabricksTypeMappingSource>());
            services.Replace(ServiceDescriptor.Scoped<IRelationalConnection, DatabricksRelationalConnection>());
            services.Replace(ServiceDescriptor.Singleton<IQuerySqlGeneratorFactory, DatabricksQuerySqlGeneratorFactory>());
            services.TryAddSingleton<IDatabaseProvider, DatabaseProvider<DatabricksOptionsExtension>>();
            services.Replace(ServiceDescriptor.Singleton<IModificationCommandBatchFactory, DatabricksModificationCommandBatchFactory>());
            services.Replace(ServiceDescriptor.Scoped<IRelationalDatabaseCreator, DatabricksDatabaseCreator>());
            services.Replace(ServiceDescriptor.Singleton<LoggingDefinitions, DatabricksLoggingDefinitions>());
            services.Replace(ServiceDescriptor.Singleton<IModelRuntimeInitializer, RelationalModelRuntimeInitializer>());
        }

        public override void Validate(IDbContextOptions options)
        {
            // Connection string is guaranteed to be non-null due to constructor validation
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new InvalidOperationException("A valid connection string must be provided.");
            }
        }

        public override DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

        protected override RelationalOptionsExtension Clone()
        {
            return new DatabricksOptionsExtension(ConnectionString ?? throw new InvalidOperationException("ConnectionString cannot be null during clone."));
        }

        private sealed class ExtensionInfo(DatabricksOptionsExtension extension) : DbContextOptionsExtensionInfo(extension)
        {
            private readonly DatabricksOptionsExtension _extension = extension;

            public override bool IsDatabaseProvider => true;
            public override string LogFragment => $"Databricks {(string.IsNullOrEmpty(_extension.ConnectionString) ? string.Empty : "ConnectionString")}";
            public override int GetServiceProviderHashCode() => _extension.ConnectionString?.GetHashCode() ?? 0;
            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
                => other is ExtensionInfo otherInfo && _extension.ConnectionString == otherInfo._extension.ConnectionString;
            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["Databricks:ConnectionString"] = _extension.ConnectionString ?? "";
            }
        }
    }
}
