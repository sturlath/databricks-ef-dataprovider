using System.Data.Common;
using System.Data.Odbc;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides a relational connection for Databricks using ODBC.
    /// </summary>
    public sealed class DatabricksRelationalConnection : RelationalConnection
    {
        public DatabricksRelationalConnection(RelationalConnectionDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override DbConnection CreateDbConnection()
            => new OdbcConnection(ConnectionString);
    }
}
