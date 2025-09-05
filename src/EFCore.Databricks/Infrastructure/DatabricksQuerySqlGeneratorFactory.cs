using Microsoft.EntityFrameworkCore.Query;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Creates instances of <see cref="DatabricksQuerySqlGenerator"/>.
    /// </summary>
    public sealed class DatabricksQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private readonly QuerySqlGeneratorDependencies _dependencies;

        public DatabricksQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies)
        {
            _dependencies = dependencies;
        }

        public QuerySqlGenerator Create() => new DatabricksQuerySqlGenerator(_dependencies);
    }
}
