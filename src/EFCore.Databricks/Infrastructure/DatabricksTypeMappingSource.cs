using System;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides type mappings for Databricks SQL.
    /// </summary>
    public sealed class DatabricksTypeMappingSource(TypeMappingSourceDependencies dependencies, RelationalTypeMappingSourceDependencies relational) 
        : RelationalTypeMappingSource(dependencies, relational)
    {
        private static readonly BoolTypeMapping _bool = new("BOOLEAN");
        private static readonly IntTypeMapping _int = new("INTEGER");
        private static readonly LongTypeMapping _long = new("BIGINT");
        private static readonly DoubleTypeMapping _double = new("DOUBLE");
        private static readonly DecimalTypeMapping _decimal = new("DECIMAL(38,18)");
        private static readonly StringTypeMapping _string = new("STRING", System.Data.DbType.String, unicode: true, size: null);
        private static readonly ByteArrayTypeMapping _binary = new("BINARY");
        private static readonly DateTimeTypeMapping _dateTime = new("TIMESTAMP");
        private static readonly DateTimeOffsetTypeMapping _dateTimeOffset = new("TIMESTAMP");
        private static readonly GuidTypeMapping _guid = new("STRING");

        protected override RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            if (clrType != null && Nullable.GetUnderlyingType(clrType) != null)
            {
                clrType = Nullable.GetUnderlyingType(clrType)!;
            }

            return clrType switch
            {
                var t when t == typeof(string) => _string,
                var t when t == typeof(int) => _int,
                var t when t == typeof(long) => _long,
                var t when t == typeof(bool) => _bool,
                var t when t == typeof(double) || t == typeof(float) => _double,
                var t when t == typeof(decimal) => _decimal,
                var t when t == typeof(DateTime) => _dateTime,
                var t when t == typeof(DateTimeOffset) => _dateTimeOffset,
                var t when t == typeof(Guid) => _guid,
                var t when t == typeof(byte[]) => _binary,
                _ => base.FindMapping(mappingInfo)
            };
        }
    }
}
