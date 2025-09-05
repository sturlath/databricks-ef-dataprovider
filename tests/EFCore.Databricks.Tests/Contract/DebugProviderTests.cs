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
            
            // Debug: Let's see what tables are in the relational model
            var tables = relationalModel.Tables;
            var tableCount = tables.Count();
            
            // This should help us understand what's missing
            // If tableCount is 0, that's our problem
            Assert.True(tableCount > 0, $"No tables found in relational model. Count: {tableCount}");
        }
        
        [Fact]
        public void Can_Get_Query_String()
        {
            using var ctx = new SimpleTestContext();
            
            // Try the simplest possible query first
            var entityType = ctx.Model.FindEntityType(typeof(SimpleEntity));
            Assert.NotNull(entityType);
            
            var relationalModel = ctx.Model.GetRelationalModel();
            var tables = relationalModel.Tables;
            var tableCount = tables.Count();
            Assert.True(tableCount > 0, $"No tables found in relational model. Count: {tableCount}");
            
            // Check table mappings for the entity
            var tableMappings = entityType.GetTableMappings();
            var mappingCount = tableMappings.Count();
            
            if (mappingCount == 0)
            {
                // This is our problem! The entity is not mapped to any table
                // Let's see what we can find out about the entity
                var tableName = entityType.GetTableName();
                var schema = entityType.GetSchema();
                
                Assert.Fail($"Entity {entityType.Name} has no table mappings. TableName: {tableName}, Schema: {schema}, Tables in model: {string.Join(", ", tables.Select(t => t.Name))}");
            }
            
            // If we get here, we have table mappings
            var entityTable = tableMappings.First().Table;
            Assert.NotNull(entityTable);
            
            // Now try the query that's failing
            var query = ctx.Entities.Select(e => e.Name);
            var sql = query.ToQueryString();
            
            Assert.NotNull(sql);
            Assert.Contains("SELECT", sql.ToUpperInvariant());
        }
    }
}