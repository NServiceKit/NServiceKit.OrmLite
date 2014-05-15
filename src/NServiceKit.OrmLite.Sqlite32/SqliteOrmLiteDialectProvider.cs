using System.Data;
using System.Data.SQLite;

namespace NServiceKit.OrmLite.Sqlite
{
    /// <summary>A sqlite ORM lite dialect provider.</summary>
    public class SqliteOrmLiteDialectProvider : SqliteOrmLiteDialectProviderBase
    {
        /// <summary>
        /// what's the purpose of this Instance field? (It's like a pseudo-wanna-be singleton?)
        /// </summary>
        public static SqliteOrmLiteDialectProvider Instance = new SqliteOrmLiteDialectProvider();

        /// <summary>Creates a connection.</summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>The new connection.</returns>
        protected override IDbConnection CreateConnection(string connectionString)
        {
            return new SQLiteConnection(connectionString, parseViaFramework: ParseViaFramework);
        }

        /// <summary>With password.</summary>
        /// <param name="password">The password.</param>
        /// <returns>A SqliteOrmLiteDialectProvider.</returns>
        public SqliteOrmLiteDialectProvider WithPassword(string password)
        {
            Password = password;
            return Instance;
        }

        /// <summary>With UTF encoding.</summary>
        /// <returns>A SqliteOrmLiteDialectProvider.</returns>
        public SqliteOrmLiteDialectProvider WithUTFEncoding()
        {
            UTF8Encoded = true;
            return Instance;
        }
    }
}