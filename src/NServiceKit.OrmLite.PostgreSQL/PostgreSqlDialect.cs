using NServiceKit.OrmLite.PostgreSQL;

namespace NServiceKit.OrmLite
{
    /// <summary>A postgre SQL dialect.</summary>
    public static class PostgreSqlDialect
    {
        /// <summary>Gets the provider.</summary>
        /// <value>The provider.</value>
        public static IOrmLiteDialectProvider Provider { get { return PostgreSQLDialectProvider.Instance; } }
    }
}