using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;

namespace EFCore.Databricks.Infrastructure
{
    /// <summary>
    /// Convention set builder for Databricks provider.
    /// </summary>
    public sealed class DatabricksConventionSetBuilder : RelationalConventionSetBuilder
    {
        public DatabricksConventionSetBuilder(
            ProviderConventionSetBuilderDependencies dependencies,
            RelationalConventionSetBuilderDependencies relationalDependencies)
            : base(dependencies, relationalDependencies)
        {
        }

        /// <summary>
        /// Builds and returns the convention set for the Databricks provider.
        /// For a read-only provider, we use the base relational conventions without modifications.
        /// </summary>
        public override ConventionSet CreateConventionSet()
        {
            // For a read-only provider, the base relational conventions are sufficient
            return base.CreateConventionSet();
        }
    }
}