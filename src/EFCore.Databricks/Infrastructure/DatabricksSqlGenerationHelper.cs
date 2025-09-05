using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Generates Databricks compatible SQL fragments.
    /// </summary>
    public sealed class DatabricksSqlGenerationHelper : RelationalSqlGenerationHelper
    {
        public DatabricksSqlGenerationHelper(RelationalSqlGenerationHelperDependencies dependencies)
            : base(dependencies)
        {
        }

        public override string GenerateParameterName(string name)
            => name;

        public override string GenerateParameterNamePlaceholder(string name)
            => $"?{name}";
    }
}
