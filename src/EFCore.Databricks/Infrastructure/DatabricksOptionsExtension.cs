using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;
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
            services.AddSingleton<IDbContextOptionsExtension, DatabricksOptionsExtension>();
            services.AddSingleton<ISqlGenerationHelper, DatabricksSqlGenerationHelper>();
        }

        public void Validate(IDbContextOptions options)
        {
            // nothing yet
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
            public override string LogFragment => "";
            public override int GetServiceProviderHashCode() => 0;
            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => true;
            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
                debugInfo["Databricks:ConnectionString"] = _extension.ConnectionString ?? "";
            }
        }
    }
}
