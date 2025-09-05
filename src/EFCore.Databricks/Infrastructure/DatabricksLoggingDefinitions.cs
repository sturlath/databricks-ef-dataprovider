using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFCore.Databricks.Infrastructure
{
    /// <summary>
    /// Databricks-specific logging definitions.
    /// </summary>
    public class DatabricksLoggingDefinitions : RelationalLoggingDefinitions
    {
        // For now, this can be empty as it inherits all necessary functionality
        // from RelationalLoggingDefinitions
    }
}