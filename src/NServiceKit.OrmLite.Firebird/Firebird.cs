using NServiceKit.OrmLite.Firebird;

namespace NServiceKit.OrmLite
{
    /// <summary>A firebird dialect.</summary>
    public static class FirebirdDialect
    {
        /// <summary>Gets the provider.</summary>
        /// <value>The provider.</value>
        public static IOrmLiteDialectProvider Provider { get { return FirebirdOrmLiteDialectProvider.Instance; } }
    }
}