namespace NServiceKit.OrmLite.Oracle
{
    /// <summary>An oracle dialect.</summary>
    public class OracleDialect
    {
        /// <summary>Gets the provider.</summary>
        /// <value>The provider.</value>
        public static IOrmLiteDialectProvider Provider { get { return OracleOrmLiteDialectProvider.Instance; } }
    }
}