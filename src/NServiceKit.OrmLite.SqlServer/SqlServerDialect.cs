using NServiceKit.OrmLite.SqlServer;

namespace NServiceKit.OrmLite
{
    /// <summary>A SQL server dialect.</summary>
    public static class SqlServerDialect
    {
        /// <summary>Gets the provider.</summary>
        /// <value>The provider.</value>
        public static IOrmLiteDialectProvider Provider { get { return SqlServerOrmLiteDialectProvider.Instance; } }
    }
}