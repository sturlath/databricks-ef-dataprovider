using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFCore.Databricks.Tests.Contract
{
    public class SelectGroupByAggregateTests
    {
        private class TestContext : DbContext
        {
            public DbSet<Order> Orders => Set<Order>();
            protected override void OnConfiguring(DbContextOptionsBuilder options)
                => options.UseDatabricks("Data Source=:memory:");
                
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Order>(entity =>
                {
                    entity.HasKey(o => o.Id);
                    entity.ToTable("Orders");
                });
            }
        }

        private record Order(int Id, int CustomerId);

        [Fact]
        public void Translates_group_by_with_count()
        {
            using var ctx = new TestContext();
            var sql = ctx.Orders
                .GroupBy(o => o.CustomerId)
                .Select(g => new { g.Key, Count = g.Count() })
                .ToQueryString();

            var upper = sql.ToUpperInvariant();
            Assert.Contains("GROUP BY", upper);
            Assert.Contains("COUNT", upper);
        }
    }
}
