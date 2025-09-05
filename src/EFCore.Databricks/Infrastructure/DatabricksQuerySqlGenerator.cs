using System;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Generates Databricks compatible SQL queries.
    /// </summary>
    public sealed class DatabricksQuerySqlGenerator : QuerySqlGenerator
    {
        public DatabricksQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
            if (selectExpression.Offset != null)
            {
                throw new NotSupportedException("Not implemented for now (read-only provider)");
            }

            if (selectExpression.Limit != null)
            {
                Sql.AppendLine().Append("LIMIT ");
                Visit(selectExpression.Limit);
            }
        }
    }
}
