using System;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides type mappings for Databricks SQL.
    /// </summary>
    public sealed class DatabricksTypeMappingSource : RelationalTypeMappingSource
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

        public DatabricksTypeMappingSource(TypeMappingSourceDependencies dependencies, RelationalTypeMappingSourceDependencies relational)
            : base(dependencies, relational)
        {
        }

        protected override RelationalTypeMapping? FindMapping(in RelationalTypeMappingInfo mappingInfo)
        {
            var clrType = mappingInfo.ClrType;
            if (clrType != null && Nullable.GetUnderlyingType(clrType) != null)
            {
                clrType = Nullable.GetUnderlyingType(clrType)!;
            }
            if (clrType == typeof(string)) return _string;
            if (clrType == typeof(int)) return _int;
            if (clrType == typeof(long)) return _long;
            if (clrType == typeof(bool)) return _bool;
            if (clrType == typeof(double) || clrType == typeof(float)) return _double;
            if (clrType == typeof(decimal)) return _decimal;
            if (clrType == typeof(DateTime)) return _dateTime;
            if (clrType == typeof(DateTimeOffset)) return _dateTimeOffset;
            if (clrType == typeof(Guid)) return _guid;
            if (clrType == typeof(byte[])) return _binary;

            return base.FindMapping(mappingInfo);
        }
    }
}
