using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace EFCore.Databricks.Infrastructure
{
    /// <summary>
    /// Generates Databricks compatible SQL queries.
    /// </summary>
    public sealed class DatabricksQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies) 
        : QuerySqlGenerator(dependencies)
    {
        protected override void GenerateLimitOffset(SelectExpression selectExpression)
        {
            if (selectExpression.Offset != null)
            {
                throw new NotSupportedException("OFFSET clause is not supported by the Databricks provider");
            }

            if (selectExpression.Limit != null)
            {
                Sql.AppendLine().Append("LIMIT ");
                Visit(selectExpression.Limit);
            }
        }
    }
}
