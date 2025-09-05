using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// EF Core options extension holding Databricks specific settings.
    /// </summary>
    public sealed class DatabricksOptionsExtension : IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo? _info;

        public string? ConnectionString { get; private set; }

        public DatabricksOptionsExtension()
        {
        }

        public DatabricksOptionsExtension WithConnectionString(string connectionString)
        {
            var clone = (DatabricksOptionsExtension)MemberwiseClone();
            clone.ConnectionString = connectionString;
            return clone;
        }

        public void ApplyServices(IServiceCollection services)
        {
            new EntityFrameworkRelationalServicesBuilder(services).TryAddCoreServices();

            services.AddSingleton<IDbContextOptionsExtension, DatabricksOptionsExtension>();
            services.AddSingleton<ISqlGenerationHelper, DatabricksSqlGenerationHelper>();
            services.AddSingleton<ITypeMappingSource, DatabricksTypeMappingSource>();
            services.AddScoped<IRelationalConnection, DatabricksRelationalConnection>();
            services.AddSingleton<IQuerySqlGeneratorFactory, DatabricksQuerySqlGeneratorFactory>();
            services.AddSingleton<IParameterNameGeneratorFactory, SequentialParameterNameGeneratorFactory>();
            services.AddSingleton<IDatabaseProvider, DatabaseProvider<DatabricksOptionsExtension>>();

            var initializerInterface = Type.GetType("Microsoft.EntityFrameworkCore.Internal.ISingletonOptionsInitializer, Microsoft.EntityFrameworkCore");
            var initializerImplementation = Type.GetType("Microsoft.EntityFrameworkCore.Internal.SingletonOptionsInitializer, Microsoft.EntityFrameworkCore");
            if (initializerInterface != null && initializerImplementation != null)
            {
                services.AddSingleton(initializerInterface, initializerImplementation);
            }
        }

        public void Validate(IDbContextOptions options)
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
            {
                throw new InvalidOperationException("A valid connection string must be provided.");
            }
        }

        public DbContextOptionsExtensionInfo Info => _info ??= new ExtensionInfo(this);

        private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            private readonly DatabricksOptionsExtension _extension;

            public ExtensionInfo(DatabricksOptionsExtension extension) : base(extension)
            {
                _extension = extension;
            }

            public override bool IsDatabaseProvider => true;
            public override string LogFragment => $"Databricks " + (string.IsNullOrEmpty(_extension.ConnectionString) ? string.Empty : "ConnectionString");
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
