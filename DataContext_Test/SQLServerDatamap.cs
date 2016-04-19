using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DataContext_Test
{
    public static class SQLServerDatamap
    {
        private static Dictionary<Type, DbType> CSharpMap = new Dictionary<Type,DbType>();
        private static Dictionary<DbType, Type> SqlServerMap = new Dictionary<DbType, Type>();
        static SQLServerDatamap()
        {
            GenerateCSharpMap();
            GenerateSqlServerMap();
        }

        public static DbType GetType(Type type)
        {
            return CSharpMap[type];
        }

        public static Type GetType(DbType type)
        {
            return SqlServerMap[type];
        }

        private static void GenerateCSharpMap()
        {
            CSharpMap[typeof(byte)] = DbType.Byte;
            CSharpMap[typeof(sbyte)] = DbType.SByte;
            CSharpMap[typeof(short)] = DbType.Int16;
            CSharpMap[typeof(ushort)] = DbType.UInt16;
            CSharpMap[typeof(int)] = DbType.Int32;
            CSharpMap[typeof(uint)] = DbType.UInt32;
            CSharpMap[typeof(long)] = DbType.Int64;
            CSharpMap[typeof(ulong)] = DbType.UInt64;
            CSharpMap[typeof(float)] = DbType.Single;
            CSharpMap[typeof(double)] = DbType.Double;
            CSharpMap[typeof(decimal)] = DbType.Decimal;
            CSharpMap[typeof(bool)] = DbType.Boolean;
            CSharpMap[typeof(string)] = DbType.String;
            CSharpMap[typeof(char)] = DbType.StringFixedLength;
            CSharpMap[typeof(Guid)] = DbType.Guid;
            CSharpMap[typeof(DateTime)] = DbType.DateTime;
            CSharpMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            CSharpMap[typeof(byte[])] = DbType.Binary;
            CSharpMap[typeof(byte?)] = DbType.Byte;
            CSharpMap[typeof(sbyte?)] = DbType.SByte;
            CSharpMap[typeof(short?)] = DbType.Int16;
            CSharpMap[typeof(ushort?)] = DbType.UInt16;
            CSharpMap[typeof(int?)] = DbType.Int32;
            CSharpMap[typeof(uint?)] = DbType.UInt32;
            CSharpMap[typeof(long?)] = DbType.Int64;
            CSharpMap[typeof(ulong?)] = DbType.UInt64;
            CSharpMap[typeof(float?)] = DbType.Single;
            CSharpMap[typeof(double?)] = DbType.Double;
            CSharpMap[typeof(decimal?)] = DbType.Decimal;
            CSharpMap[typeof(bool?)] = DbType.Boolean;
            CSharpMap[typeof(char?)] = DbType.StringFixedLength;
            CSharpMap[typeof(Guid?)] = DbType.Guid;
            CSharpMap[typeof(DateTime?)] = DbType.DateTime;
            CSharpMap[typeof(DateTimeOffset?)] = DbType.DateTimeOffset;
            CSharpMap[typeof(System.Data.Linq.Binary)] = DbType.Binary;
        }
        private static void GenerateSqlServerMap()
        {
            SqlServerMap[DbType.Byte] = typeof(byte);
            SqlServerMap[DbType.SByte] = typeof(sbyte);
            SqlServerMap[DbType.Int16] = typeof(short);
            SqlServerMap[DbType.UInt16] = typeof(ushort);
            SqlServerMap[DbType.Int32] = typeof(int);
            SqlServerMap[DbType.UInt32] = typeof(uint);
            SqlServerMap[DbType.Int64] = typeof(long);
            SqlServerMap[DbType.UInt64] = typeof(ulong);
            SqlServerMap[DbType.Single] = typeof(float);
            SqlServerMap[DbType.Double] = typeof(double);
            SqlServerMap[DbType.Decimal] = typeof(decimal);
            SqlServerMap[DbType.Boolean] = typeof(bool);
            SqlServerMap[DbType.String] = typeof(string);
            SqlServerMap[DbType.StringFixedLength] = typeof(char);
            SqlServerMap[DbType.Guid] = typeof(Guid);
            SqlServerMap[DbType.DateTime] = typeof(DateTime);
            SqlServerMap[DbType.DateTimeOffset] = typeof(DateTimeOffset);
            SqlServerMap[DbType.Binary] = typeof(byte[]);
        }
    }
}
