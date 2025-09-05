using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFCore.Databricks.Tests.Contract
{
    public class SelectProjectionTests
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
        public void Generates_select_projection_sql()
        {
            using var ctx = new TestContext();
            var sql = ctx.Customers
                .Select(c => c.Name)
                .OrderBy(c => c)
                .Take(5)
                .ToQueryString();

            Assert.Contains("LIMIT", sql.ToUpperInvariant());
        }
    }
}
