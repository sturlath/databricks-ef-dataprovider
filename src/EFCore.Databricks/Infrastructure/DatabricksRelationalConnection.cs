using System.Data.Common;
using System.Data.Odbc;
using Microsoft.EntityFrameworkCore.Storage;

namespace EFCore.Databricks.Infrastructure
{
    /// <summary>
    /// Provides a relational connection for Databricks using ODBC.
    /// </summary>
    public sealed class DatabricksRelationalConnection(RelationalConnectionDependencies dependencies) 
        : RelationalConnection(dependencies)
    {
        protected override DbConnection CreateDbConnection()
            => new OdbcConnection(ConnectionString);
    }
}
