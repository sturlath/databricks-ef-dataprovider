using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EFCore.Databricks.Tests.Contract
{
    public class DebugProviderTests
    {
        private class SimpleTestContext : DbContext
        {
            public DbSet<SimpleEntity> Entities => Set<SimpleEntity>();
            
            protected override void OnConfiguring(DbContextOptionsBuilder options)
                => options.UseDatabricks("Data Source=:memory:");
                
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<SimpleEntity>(entity =>
                {
                    entity.HasKey(e => e.Id);
                    entity.ToTable("SimpleEntities");
                    entity.Property(e => e.Id).ValueGeneratedNever();
                    entity.Property(e => e.Name).IsRequired();
                });
            }
        }

        public record SimpleEntity(int Id, string Name);

        [Fact]
        public void Can_Create_Context()
        {
            using var ctx = new SimpleTestContext();
            
            // Just check that the context can be created
            Assert.NotNull(ctx);
            Assert.NotNull(ctx.Model);
            
            var entityType = ctx.Model.FindEntityType(typeof(SimpleEntity));
            Assert.NotNull(entityType);
            
            var relationalModel = ctx.Model.GetRelationalModel();
            Assert.NotNull(relationalModel);
        }
        
        [Fact]
        public void Can_Get_Query_String()
        {
            using var ctx = new SimpleTestContext();
            
            // Try the simplest possible query
            var query = ctx.Entities.Select(e => e.Name);
            var sql = query.ToQueryString();
            
            Assert.NotNull(sql);
            Assert.Contains("SELECT", sql.ToUpperInvariant());
        }
    }
}