using NServiceKit.OrmLite.Oracle;

namespace NServiceKit.OrmLite
{
    /// <summary>An oracle dialect.</summary>
    public static class OracleDialect
    {
        /// <summary>Gets the provider.</summary>
        /// <value>The provider.</value>
        public static IOrmLiteDialectProvider Provider { get { return OracleOrmLiteDialectProvider.Instance; } }
    }
}