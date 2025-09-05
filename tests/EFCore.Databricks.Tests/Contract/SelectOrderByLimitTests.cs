using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFCore.Databricks.Tests.Contract
{
    public class SelectOrderByLimitTests
    {
        private class TestContext : DbContext
        {
            public DbSet<Customer> Customers => Set<Customer>();
            protected override void OnConfiguring(DbContextOptionsBuilder options)
                => options.UseDatabricks("Data Source=:memory:");
                
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Customer>(entity =>
                {
                    entity.HasKey(c => c.Id);
                    entity.ToTable("Customers");
                });
            }
        }

        private record Customer(int Id, string Name);

        [Fact]
        public void Translates_order_by_and_limit()
        {
            using var ctx = new TestContext();
            var sql = ctx.Customers
                .OrderBy(c => c.Name)
                .Take(3)
                .ToQueryString();

            var upper = sql.ToUpperInvariant();
            Assert.Contains("ORDER BY", upper);
            Assert.Contains("LIMIT", upper);
        }
    }
}
