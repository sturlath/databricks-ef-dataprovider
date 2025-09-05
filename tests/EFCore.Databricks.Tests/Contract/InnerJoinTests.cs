using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFCore.Databricks.Tests.Contract
{
    public class InnerJoinTests
    {
        private class TestContext : DbContext
        {
            public DbSet<Customer> Customers => Set<Customer>();
            public DbSet<Order> Orders => Set<Order>();
            protected override void OnConfiguring(DbContextOptionsBuilder options)
                => options.UseDatabricks("Data Source=:memory:");
        }

        private record Customer(int Id, string Name);
        private record Order(int Id, int CustomerId);

        [Fact]
        public void Translates_inner_join()
        {
            using var ctx = new TestContext();
            var sql = ctx.Customers
                .Join(ctx.Orders, c => c.Id, o => o.CustomerId, (c, o) => new { c.Name, o.Id })
                .ToQueryString();

            Assert.Contains("INNER JOIN", sql.ToUpperInvariant());
        }
    }
}
