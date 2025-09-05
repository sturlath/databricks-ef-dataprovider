using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Generates Databricks compatible SQL fragments.
    /// </summary>
    public sealed class DatabricksSqlGenerationHelper(RelationalSqlGenerationHelperDependencies dependencies) 
        : RelationalSqlGenerationHelper(dependencies)
    {
        public override string GenerateParameterName(string name)
            => name;

        public override string GenerateParameterNamePlaceholder(string name)
            => "?";
    }
}
