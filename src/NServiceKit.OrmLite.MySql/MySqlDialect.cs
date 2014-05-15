using NServiceKit.OrmLite.MySql;

namespace NServiceKit.OrmLite
{
    /// <summary>my SQL dialect.</summary>
    public static class MySqlDialect
    {
        /// <summary>Gets the provider.</summary>
        /// <value>The provider.</value>
        public static IOrmLiteDialectProvider Provider { get { return MySqlDialectProvider.Instance; } }
    }
}