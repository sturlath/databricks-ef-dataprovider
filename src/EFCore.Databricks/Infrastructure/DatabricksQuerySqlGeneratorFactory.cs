using EFCore.Databricks.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Creates instances of <see cref="DatabricksQuerySqlGenerator"/>.
    /// </summary>
    public sealed class DatabricksQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies) : IQuerySqlGeneratorFactory
    {
        public QuerySqlGenerator Create() => new DatabricksQuerySqlGenerator(dependencies);
    }
}
