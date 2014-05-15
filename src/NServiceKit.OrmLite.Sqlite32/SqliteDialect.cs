using NServiceKit.OrmLite.Sqlite;

namespace NServiceKit.OrmLite
{
    /// <summary>A sqlite dialect.</summary>
    public static class SqliteDialect
    {
        /// <summary>Gets the provider.</summary>
        /// <value>The provider.</value>
        public static IOrmLiteDialectProvider Provider { get { return SqliteOrmLiteDialectProvider.Instance; } }
    }
}