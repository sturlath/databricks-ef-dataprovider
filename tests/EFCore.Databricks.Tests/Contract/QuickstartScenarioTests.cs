using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFCore.Databricks.Tests.Contract
{
    public class QuickstartScenarioTests
    {
        private class TestContext : DbContext
        {
            public DbSet<Customer> Customers => Set<Customer>();
            protected override void OnConfiguring(DbContextOptionsBuilder options)
                => options.UseDatabricks();
        }

        private record Customer(int Id, string Name);

        [Fact]
        public void Configures_via_environment_variables()
        {
            Environment.SetEnvironmentVariable("DATABRICKS_CONNECTION_STRING", "Data Source=:memory:");
            using var ctx = new TestContext();

            Assert.Equal(QueryTrackingBehavior.NoTracking, ctx.ChangeTracker.QueryTrackingBehavior);

            var sql = ctx.Customers.OrderBy(c => c.Id).Take(1).ToQueryString();
            Assert.Contains("LIMIT", sql.ToUpperInvariant());
        }
    }
}
