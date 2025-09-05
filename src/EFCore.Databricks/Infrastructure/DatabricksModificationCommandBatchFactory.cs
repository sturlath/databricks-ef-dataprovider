using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Update;

namespace EFCore.Databricks.Infrastructure
{
    /// <summary>
    /// Databricks modification command batch factory.
    /// Since Databricks is typically used for read-only operations,
    /// this provides minimal update functionality.
    /// </summary>
    public sealed class DatabricksModificationCommandBatchFactory : IModificationCommandBatchFactory
    {
        public ModificationCommandBatch Create()
            => throw new NotSupportedException("Databricks provider does not support modification operations.");
    }
}