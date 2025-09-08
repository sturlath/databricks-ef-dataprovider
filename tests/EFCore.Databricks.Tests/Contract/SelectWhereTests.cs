using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFCore.Databricks.Tests.Contract
{
    public class SelectWhereTests
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
        public void Translates_where_clause_with_parameters()
        {
            using var ctx = new TestContext();
            var id = 10;
            var name = "foo";
            var sql = ctx.Customers
                .Where(c => c.Id > id && c.Name == name)
                .ToQueryString();

            Assert.Contains("WHERE", sql.ToUpperInvariant());
            // Parameter declarations are emitted as SQL comments by EF Core's ToQueryString
            Assert.Contains("__id_0='10'", sql);
            Assert.Contains("__name_1='foo'", sql);
            // Ensure positional parameter placeholders are present
            Assert.Contains("?", sql);
        }
    }
}
