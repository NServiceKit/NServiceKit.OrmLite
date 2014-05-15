using System;
using System.Data;
using System.IO;
using NUnit.Framework;
using NServiceKit.Common.Utils;
using NServiceKit.Logging;
using NServiceKit.Logging.Support.Logging;
using NServiceKit.OrmLite.Sqlite;

namespace NServiceKit.OrmLite.Tests
{
    /// <summary>A configuration.</summary>
    public class Config
    {
        /// <summary>The sqlite memory database.</summary>
        public static string SqliteMemoryDb = ":memory:";

        /// <summary>The sqlite file dir.</summary>
        public static string SqliteFileDir = "~/App_Data/".MapAbsolutePath();

        /// <summary>The sqlite file database.</summary>
        public static string SqliteFileDb = "~/App_Data/db.sqlite".MapAbsolutePath();
    }

    /// <summary>An ORM lite test base.</summary>
	public class OrmLiteTestBase
	{
        /// <summary>Gets or sets the connection string.</summary>
        /// <value>The connection string.</value>
		protected virtual string ConnectionString { get; set; }

        /// <summary>Gets file connection string.</summary>
        /// <returns>The file connection string.</returns>
		protected virtual string GetFileConnectionString()
		{
            var connectionString = Config.SqliteFileDb;
			if (File.Exists(connectionString))
				File.Delete(connectionString);

			return connectionString;
		}

        /// <summary>Creates new database.</summary>
		protected void CreateNewDatabase()
		{
			if (ConnectionString.Contains(".sqlite"))
				ConnectionString = GetFileConnectionString();
		}

        /// <summary>Tests fixture set up.</summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			LogManager.LogFactory = new ConsoleLogFactory();

			OrmLiteConfig.DialectProvider = SqliteOrmLiteDialectProvider.Instance;
			//ConnectionString = ":memory:";
			ConnectionString = GetFileConnectionString();

			//OrmLiteConfig.DialectProvider = SqlServerOrmLiteDialectProvider.Instance;
			//ConnectionString = "~/App_Data/Database1.mdf".MapAbsolutePath();			
		}

        /// <summary>Logs.</summary>
        /// <param name="text">The text.</param>
		public void Log(string text)
		{
			Console.WriteLine(text);
		}

        /// <summary>Gets or sets the in memory database connection.</summary>
        /// <value>The in memory database connection.</value>
        public IDbConnection InMemoryDbConnection { get; set; }

        /// <summary>Opens database connection.</summary>
        /// <param name="connString">The connection string.</param>
        /// <returns>An IDbConnection.</returns>
        public IDbConnection OpenDbConnection(string connString = null)
        {
            connString = connString ?? ConnectionString;
            if (connString == ":memory:")
            {
                if (InMemoryDbConnection == null)
                {
                    var dbConn = connString.OpenDbConnection();
                    InMemoryDbConnection = new OrmLiteConnectionWrapper(dbConn)
                    {
                        DialectProvider = OrmLiteConfig.DialectProvider,
                        AutoDisposeConnection = false,
                    };
                }

                return InMemoryDbConnection;
            }

            return connString.OpenDbConnection();
        }
    }
}